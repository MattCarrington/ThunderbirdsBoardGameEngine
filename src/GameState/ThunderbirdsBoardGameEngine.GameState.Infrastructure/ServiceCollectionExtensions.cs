using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameState(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateGameSessionHandler).Assembly);

            services.AddSingleton<IGameSessionRespository, InMemoryGameSessionRepository>();

            return services;
        }
    }
}
