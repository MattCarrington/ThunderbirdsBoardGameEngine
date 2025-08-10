using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameData.Application.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Application.Services;

namespace ThunderbirdsBoardGameEngine.GameData.Application
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddScoped<IDisasterCardService, DisasterCardService>();
            return services;
        }
    }
}
