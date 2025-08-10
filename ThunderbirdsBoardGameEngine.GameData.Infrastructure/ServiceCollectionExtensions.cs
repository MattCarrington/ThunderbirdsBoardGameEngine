using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.GameData.Application.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.GameData.Infrastructure.Repositories;

namespace ThunderbirdsBoardGameEngine.GameData.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CardDataOptions>(
                configuration.GetSection("CardData"));

            services.AddScoped<IDisasterCardRepository, DisasterCardRepository>();

            return services;
        }
    }
}
