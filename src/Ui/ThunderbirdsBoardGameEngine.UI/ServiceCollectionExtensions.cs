using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Services;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Mappers;
using ThunderbirdsBoardGameEngine.UI.Services;

namespace ThunderbirdsBoardGameEngine.UI
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddUiServices(this IServiceCollection services)
        {
            services.AddScoped<IDisasterCardService, DisasterCardService>();
            services.AddScoped<ICharacterService, CharacterService>();
            services.AddScoped<IRescueService, RescueService>();
            services.AddScoped<IThunderbirdMovementOptionsService, ThunderbirdMovementOptionsService>();
            services.AddScoped<IMovementClientService, MovementClientService>();

            services.AddSingleton<DisasterCardMapper>();
            services.AddSingleton<CharacterMapper>();
            services.AddSingleton<MovementResultMapper>();
            services.AddSingleton<MovementLocationOptionsMapper>();

            return services;
        }
    }
}
