using Pulumi;
using Pulumi.AzureNative.App;
using Pulumi.AzureNative.App.Inputs;
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
    const string registryPasswordSecretName = "registry-password";

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
        Tags = tags,
    });

    // Build registries and secrets lists without nulls
    var registries = new List<RegistryCredentialsArgs>();
    var secrets = new List<SecretArgs>();
    if (!string.IsNullOrWhiteSpace(registryUsername) && registryPassword != null)
    {
        var registryServer = TryGetRegistryServer(containerImage) ?? throw new InvalidOperationException(
            "When using registry credentials, thunderbirds-azure-dev:containerImage must include an explicit registry server (for example, ghcr.io/owner/image:tag or myregistry.azurecr.io/image:tag).");

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
        },
        Template = new TemplateArgs
        {
            Containers =
            {
                new ContainerArgs
                {
                    Name = "app",
                    Image = containerImage,
                    Resources = new ContainerResourcesArgs
                    {
                        Cpu = 0.25,
                        Memory = "0.5Gi",
                    },
                },
            },
            Scale = new ScaleArgs
            {
                MinReplicas = 1,
                MaxReplicas = 1,
            },
        },
        Tags = tags,
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
