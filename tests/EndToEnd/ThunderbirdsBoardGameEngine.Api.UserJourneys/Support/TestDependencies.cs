using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.Support
{
    public static class TestDependencies
    {
        [ScenarioDependencies]
        public static IServiceCollection CreateServices()
        {
            var configuration = new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();

            var services = new ServiceCollection();

            services.AddSingleton<IConfiguration>(configuration);

            services.AddGameStateClients(configuration);

            services.AddSingleton<GameJourneyContext>();

            services.AddSingleton<IApplicationRestarter, DockerApplicationRestarter>();

            return services;
        }
    }
}