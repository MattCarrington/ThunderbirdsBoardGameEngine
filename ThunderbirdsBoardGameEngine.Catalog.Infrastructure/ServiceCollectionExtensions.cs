using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<CardDataOptions>(
                configuration.GetSection("CardData"));

            services.AddScoped<IDisasterCardRepository, JsonDisasterCardRepository>();

            return services;
        }
    }
}
