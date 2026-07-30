using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Infrastructure;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Fixtures;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.Helpers
{
    public sealed class GameStateTestHost : IAsyncDisposable
    {
        private readonly ServiceProvider _serviceProvider;
        private readonly AsyncServiceScope _scope;

        private GameStateTestHost(ServiceProvider serviceProvider, AsyncServiceScope scope)
        {
            _serviceProvider = serviceProvider;
            _scope = scope;
        }

        public IMediator Mediator => _serviceProvider.GetRequiredService<IMediator>();

        public InMemoryGameRepository Repository => _serviceProvider.GetRequiredService<InMemoryGameRepository>();

        public T GetService<T>() where T : notnull
        {
            return _serviceProvider.GetRequiredService<T>();
        }

        public async ValueTask DisposeAsync()
        {
            await _scope.DisposeAsync();
            await _serviceProvider.DisposeAsync();
        }

        public static GameStateTestHostBuilder CreateBuilder()
        {
            return new GameStateTestHostBuilder();
        }

        public sealed class GameStateTestHostBuilder
        {
            private readonly ServiceCollection _services = new();
            private InMemoryGameRepository? _repository;

            public GameStateTestHostBuilder()
            {
                _services.AddGameState();
                _services.AddGameStateIntegrity();
                _services.AddFakeCatalogs();
            }

            public GameStateTestHostBuilder WithRepository(InMemoryGameRepository repository)
            {
                _repository = repository;
                return this;
            }

            public GameStateTestHostBuilder WithService<TService>(TService implementation) where TService : class
            {
                _services.AddSingleton(implementation);
                return this;
            }

            public GameStateTestHostBuilder WithService<TService, TImplementation>()
                where TService : class
                where TImplementation : class, TService
            {
                _services.AddSingleton<TService, TImplementation>();
                return this;
            }

            public GameStateTestHost Build()
            {
                var repository = _repository ?? new InMemoryGameRepository();
                _services.AddSingleton<IGameRepository>(repository);
                _services.AddSingleton(repository);

                var serviceProvider = _services.BuildServiceProvider(
                    new ServiceProviderOptions
                    {
                        ValidateScopes = true
                    });

                var scope = serviceProvider.CreateAsyncScope();

                return new GameStateTestHost(serviceProvider, scope);
            }
        }
    }
}
