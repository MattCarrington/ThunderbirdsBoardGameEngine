using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameState(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateGameSessionHandler).Assembly);

            services.AddSingleton<IGameSessionRespository, InMemoryGameSessionRepository>();
            services.AddSingleton<IValidateMovementGateway, ValidateMovementGateway>();

            return services;
        }
    }
}
