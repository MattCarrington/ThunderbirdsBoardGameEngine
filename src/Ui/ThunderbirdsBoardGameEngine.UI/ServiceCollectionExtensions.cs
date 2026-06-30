using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Services;

namespace ThunderbirdsBoardGameEngine.UI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUiServices(this IServiceCollection services)
        {
            services.AddScoped<IDisasterCardService, DisasterCardService>();
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IRescueCalculationModifierService, RescueCalculationModifierService>();
            services.AddScoped<IRescueClientService, RescueClientService>();
            services.AddScoped<IThunderbirdMovementOptionsService, ThunderbirdMovementOptionsService>();
            services.AddScoped<IMovementClientService, MovementClientService>();

            services.AddSingleton<DisasterCardMapper>();
            services.AddSingleton<MovementResultMapper>();
            services.AddSingleton<MovementLocationOptionsMapper>();

            services.AddGameSession();

            return services;
        }

        private static IServiceCollection AddGameSession(this IServiceCollection services)
        {
            services.AddScoped<IGameService, GameService>();
            return services;
        }
    }
}
