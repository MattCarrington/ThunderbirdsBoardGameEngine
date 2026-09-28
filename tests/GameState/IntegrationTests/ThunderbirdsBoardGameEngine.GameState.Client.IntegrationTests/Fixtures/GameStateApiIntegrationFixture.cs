using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Client.Extensions;
using ThunderbirdsBoardGameEngine.GameState.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.Client.IntegrationTests.Fixtures
{
    public sealed class GameStateApiIntegrationFixture : IAsyncLifetime
    {
        private ServiceProvider _serviceProvider = null!;

        public IGameClient Client { get; private set; } = null!;

        public ValueTask InitializeAsync()
        {
            var configuration = BuildConfiguration();

            var services = new ServiceCollection();

            services.AddGameStateClients(configuration);
            services.AddGameStatePersistence(configuration);

            _serviceProvider = services.BuildServiceProvider(
                new ServiceProviderOptions
                {
                    ValidateScopes = true,
                    ValidateOnBuild = true
                });

            Client = _serviceProvider.GetRequiredService<IGameClient>();

            return ValueTask.CompletedTask;
        }

        public async Task<Game> SeedGame(CancellationToken cancellationToken)
        {
            var game = new StandardGameSetupFactory().Create(
                Guid.NewGuid(),
                DateTimeOffset.UtcNow);

            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

            await repository.CreateNewGameSession(
                game,
                cancellationToken);

            return game;
        }

        public async Task<Game?> LoadGame(
            Guid gameId,
            CancellationToken cancellationToken)
        {
            await using var scope = _serviceProvider.CreateAsyncScope();

            var repository = scope.ServiceProvider.GetRequiredService<IGameRepository>();

            return await repository.GetGameSessionById(
                gameId,
                cancellationToken);
        }

        public async ValueTask DisposeAsync()
        {
            await _serviceProvider.DisposeAsync();
        }

        private static IConfiguration BuildConfiguration()
        {
            return new ConfigurationBuilder()
                .AddEnvironmentVariables()
                .Build();
        }
    }
}
