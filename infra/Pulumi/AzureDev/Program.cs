using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
using PostgreSql = Pulumi.AzureNative.DBforPostgreSQL;
using PostgreSqlInputs = Pulumi.AzureNative.DBforPostgreSQL.Inputs;
using Resources = Pulumi.AzureNative.Resources;

return await Deployment.RunAsync(() =>
{
    var azureConfig = new Config("azure-native");
    var projectConfig = new Config("thunderbirds-azure-dev");

    var location = azureConfig.Get("location") ?? "uksouth";
    var baseName = projectConfig.Get("baseName") ?? "tbbge";
    var containerImage = projectConfig.Get("containerImage") ?? "ghcr.io/mattcarrington/thunderbirds-app:latest";
    var containerPort = projectConfig.GetInt32("containerPort") ?? 8080;
    var registryUsername = projectConfig.Get("registryUsername");
    var registryPassword = projectConfig.GetSecret("registryPassword");
    var stack = Deployment.Instance.StackName;

    var databaseName = projectConfig.Get("databaseName") ?? "thunderbirds_game_state";
    var databaseAdministratorUsername = projectConfig.Require("databaseAdministratorUsername");
    var databaseAdministratorPassword = projectConfig.RequireSecret("databaseAdministratorPassword");

    var databaseMigratorImage = projectConfig.Get("databaseMigratorImage") ?? "ghcr.io/mattcarrington/thunderbirds-game-state-db-migrator:latest";
    var databaseProvisionerImage = projectConfig.Get("databaseProvisionerImage") ?? "ghcr.io/mattcarrington/thunderbirds-game-state-db-provisioner:latest";

    if (string.IsNullOrWhiteSpace(databaseName))
    {
        throw new InvalidOperationException("thunderbirds-azure-dev:databaseName cannot be empty.");
    }

    if (string.IsNullOrWhiteSpace(databaseAdministratorUsername))
    {
        throw new InvalidOperationException(
            "thunderbirds-azure-dev:databaseAdministratorUsername " +
            "cannot be empty.");
    }

    const string registryPasswordSecretName = "registry-password";
    const string databaseAdministratorConnectionStringSecretName = "game-state-database-connection-string";
    const string databaseRuntimeConnectionStringSecretName = "game-state-runtime-database-connection-string";
    const string databaseConnectionStringEnvironmentVariable = "GameState__Persistence__ConnectionString";
    const string databaseRuntimePasswordSecretName = "game-state-runtime-password";

    var tags = BuildTags(stack);

    var resourceGroup = new Resources.ResourceGroup($"{baseName}-{stack}-rg", new Resources.ResourceGroupArgs
    {
        Location = location,
        Tags = tags,
    });

    var containerAppEnvironment = new ManagedEnvironment($"{baseName}-{stack}-cae", new ManagedEnvironmentArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        PublicNetworkAccess = "Enabled",
        PeerAuthentication = new ManagedEnvironmentPeerAuthenticationArgs
        {
            Mtls = new MtlsArgs
            {
                Enabled = false,
            }
        },
        PeerTrafficConfiguration = new ManagedEnvironmentPeerTrafficConfigurationArgs
        {
            Encryption = new ManagedEnvironmentEncryptionArgs
            {
                Enabled = false
            }
        },
        Tags = tags,
    });

    var databaseServer = new PostgreSql.Server($"{baseName}-{stack}-postgres", new PostgreSql.ServerArgs
    {
        ResourceGroupName = resourceGroup.Name,
        Location = resourceGroup.Location,
        AdministratorLogin = databaseAdministratorUsername,
        AdministratorLoginPassword = databaseAdministratorPassword,
        Version = PostgreSql.PostgresMajorVersion.PostgresMajorVersion_16,
        Network = new PostgreSqlInputs.NetworkArgs
        {
            PublicNetworkAccess = PostgreSql.ServerPublicNetworkAccessState.Enabled,
        },
        Sku = new PostgreSqlInputs.SkuArgs
        {
            Name = "Standard_B1ms",
            Tier = PostgreSql.SkuTier.Burstable,
        },
        Storage = new PostgreSqlInputs.StorageArgs
        {
            StorageSizeGB = 32,
        },
        Tags = tags,
    });

    var allowAzureServices = new PostgreSql.FirewallRule($"{baseName}-{stack}-postgres-allow-azure", new PostgreSql.FirewallRuleArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ServerName = databaseServer.Name,
        FirewallRuleName = "AllowAzureServices",
        StartIpAddress = "0.0.0.0",
        EndIpAddress = "0.0.0.0",
    });

    var database = new PostgreSql.Database($"{baseName}-{stack}-{databaseName}", new PostgreSql.DatabaseArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ServerName = databaseServer.Name,
        DatabaseName = databaseName,
        Charset = "UTF8",
        Collation = "en_US.utf8",
    });

    var databaseRuntimeUsername = projectConfig.Require("databaseRuntimeUsername");
    var databaseRuntimePassword = projectConfig.RequireSecret("databaseRuntimePassword");

    var databaseRuntimeConnectionString = Output.Format(
        $"Host={databaseServer.FullyQualifiedDomainName};Port=5432;Database={database.Name};Username={databaseRuntimeUsername};Password={databaseRuntimePassword};SSL Mode=Require");

    var databaseAdministratorConnectionString = Output.Format(
        $"Host={databaseServer.FullyQualifiedDomainName};Port=5432;Database={database.Name};Username={databaseAdministratorUsername};Password={databaseAdministratorPassword};SSL Mode=Require");

    // Build registries and secrets lists without nulls
    var registries = new List<RegistryCredentialsArgs>();

    var secrets = new List<SecretArgs>
    {
        new()
        {
            Name = databaseAdministratorConnectionStringSecretName,
            Value = databaseAdministratorConnectionString,
        },
        new()
        {
            Name = databaseRuntimeConnectionStringSecretName,
            Value = databaseRuntimeConnectionString,
        },
        new()
        {
            Name = databaseRuntimePasswordSecretName,
            Value = databaseRuntimePassword,
        },
    };
    if (!string.IsNullOrWhiteSpace(registryUsername) && registryPassword != null)
    {
        var registryServer = TryGetRegistryServer(containerImage)
            ?? throw new InvalidOperationException(
                "When using registry credentials, thunderbirds-azure-dev:containerImage must include an explicit registry server.");

        registries.Add(new RegistryCredentialsArgs
        {
            Server = registryServer,
            Username = registryUsername,
            PasswordSecretRef = registryPasswordSecretName,
        });

        secrets.Add(new SecretArgs
        {
            Name = registryPasswordSecretName,
            Value = registryPassword,
        });
    }
    else if (!string.IsNullOrWhiteSpace(registryUsername) || registryPassword != null)
    {
        throw new InvalidOperationException(
            "Both thunderbirds-azure-dev:registryUsername and thunderbirds-azure-dev:registryPassword must be set together for private registry authentication.");
    }

    var containerApp = new ContainerApp($"{baseName}-{stack}-app", new ContainerAppArgs
    {
        ResourceGroupName = resourceGroup.Name,
        ManagedEnvironmentId = containerAppEnvironment.Id,
        Configuration = new ConfigurationArgs
        {
            Ingress = new IngressArgs
            {
                External = true,
                TargetPort = containerPort,
                Transport = IngressTransportMethod.Auto,
            },
            Registries = registries,
            Secrets = secrets,
            IdentitySettings = [],
        },
        Template = new TemplateArgs
        {
            InitContainers =
            {
                new InitContainerArgs
                {
                    Name = "game-state-db-migrator",
                    Image = databaseMigratorImage,
                    Env =
                    {
                        new EnvironmentVarArgs
                        {
                            Name = databaseConnectionStringEnvironmentVariable,
                            SecretRef = databaseAdministratorConnectionStringSecretName,
                        },
                    },
                    Resources = new ContainerResourcesArgs
                    {
                        Cpu = 0.25,
                        Memory = "0.5Gi",
                    },
                    ImageType = ImageType.ContainerImage,
                },
                new InitContainerArgs
                {
                    Name = "game-state-db-provisioner",
                    Image = databaseProvisionerImage,
                    Env =
                    {
                        new EnvironmentVarArgs
                        {
                            Name = "GameState__DatabaseProvisioning__AdministratorConnectionString",
                            SecretRef = databaseAdministratorConnectionStringSecretName,
                        },
                        new EnvironmentVarArgs
                        {
                            Name = "GameState__DatabaseProvisioning__RuntimeUsername",
                            Value = databaseRuntimeUsername,
                        },
                        new EnvironmentVarArgs
                        {
                            Name = "GameState__DatabaseProvisioning__RuntimePassword",
                            SecretRef = databaseRuntimePasswordSecretName,
                        },
                    },
                    Resources = new ContainerResourcesArgs
                    {
                        Cpu = 0.25,
                        Memory = "0.5Gi",
                    },
                    ImageType = ImageType.ContainerImage,
                }
            },
            Containers =
            {
                new ContainerArgs
                {
                    Name = "app",
                    Image = containerImage,
                    Env =
                    {
                        new EnvironmentVarArgs
                        {
                            Name = databaseConnectionStringEnvironmentVariable,
                            SecretRef = databaseRuntimeConnectionStringSecretName,
                        },
                    },
                    Resources = new ContainerResourcesArgs
                    {
                        Cpu = 0.25,
                        Memory = "0.5Gi",
                    },
                    ImageType = ImageType.ContainerImage,
                }
            },
            Scale = new ScaleArgs
            {
                MinReplicas = 1,
                MaxReplicas = 1,
                CooldownPeriod = 300,
                PollingInterval = 30,
            },
        },
        Tags = tags,
    },
    new CustomResourceOptions
    {
        IgnoreChanges =
        {
            "configuration.targetLabel",
        },
    });

    return new Dictionary<string, object?>
    {
        ["resourceGroupName"] = resourceGroup.Name,
        ["containerAppEnvironmentName"] = containerAppEnvironment.Name,
        ["containerAppEnvironmentDefaultDomain"] = containerAppEnvironment.DefaultDomain,
        ["containerAppName"] = containerApp.Name,
        ["containerImage"] = containerImage,
        ["applicationUrl"] = containerApp.LatestRevisionFqdn.Apply(static fqdn => fqdn is null ? null : $"https://{fqdn}"),
    };
});

static InputMap<string> BuildTags(string stackName)
{
    return new InputMap<string>
    {
        ["project"] = "ThunderbirdsBoardGameEngine",
        ["environment"] = stackName,
        ["managedBy"] = "Pulumi",
    };
}

static string? TryGetRegistryServer(string containerImage)
{
    if (string.IsNullOrWhiteSpace(containerImage))
    {
        return null;
    }

    var imageWithoutDigest = containerImage.Split('@', 2)[0];
    var firstSegment = imageWithoutDigest.Split('/', 2)[0];

    if (firstSegment.Contains('.') || firstSegment.Contains(':') || firstSegment.Equals("localhost", StringComparison.OrdinalIgnoreCase))
    {
        return firstSegment;
    }

    return null;
}
