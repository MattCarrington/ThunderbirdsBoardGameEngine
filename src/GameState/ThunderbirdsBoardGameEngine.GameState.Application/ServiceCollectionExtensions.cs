using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;

namespace ThunderbirdsBoardGameEngine.GameState.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddGameState(this IServiceCollection services)
        {
            services.AddMediatR(typeof(CreateNewGameHandler).Assembly);

            RegisterCreateGame(services);

            return services;
        }

        private static void RegisterCreateGame(IServiceCollection services)
        {
            services.AddSingleton<StandardGameSetupFactory>();
        }
    }
}
