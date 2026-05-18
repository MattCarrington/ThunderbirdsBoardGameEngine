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
    var containerImage = projectConfig.Get("containerImage") ?? "mcr.microsoft.com/azuredocs/containerapps-helloworld:latest";
    var containerPort = projectConfig.GetInt32("containerPort") ?? 80;
    var registryUsername = projectConfig.Get("registryUsername");
    var registryPassword = projectConfig.GetSecret("registryPassword");
    var stack = Deployment.Instance.StackName;

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
            Registries =
            {
                // Only add registry credentials if both are set
                registryUsername != null && registryPassword != null
                    ? new RegistryCredentialsArgs
                    {
                        Server = "ghcr.io",
                        Username = registryUsername,
                        PasswordSecretRef = "ghcr-password"
                    }
                    : null
            },
            Secrets =
            {
                registryPassword != null
                    ? new SecretArgs
                    {
                        Name = "ghcr-password",
                        Value = registryPassword
                    }
                    : null
            },
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
