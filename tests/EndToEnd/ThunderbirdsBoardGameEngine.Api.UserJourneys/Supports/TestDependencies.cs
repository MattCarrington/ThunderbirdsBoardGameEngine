using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Reqnroll.Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Api.UserJourneys.Supports
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

            return services;
        }
    }

    public sealed class GameJourneyContext
    {
        public Guid GameId { get; set; }

        public GameStateResponseDto? CurrentGame { get; set; }
    }
}