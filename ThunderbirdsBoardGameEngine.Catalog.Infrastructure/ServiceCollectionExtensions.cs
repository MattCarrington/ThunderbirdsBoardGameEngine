using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<CardDataOptions>()
                .Bind(configuration.GetSection("CardData"))
                .ValidateOnStart(); // ← run all validators during startup

            services.AddSingleton<IPostConfigureOptions<CardDataOptions>, CardDataOptionsPostConfigure>();
            services.AddSingleton<IValidateOptions<CardDataOptions>, CardDataOptionsFileValidator>();
            services.AddSingleton<IFileReader, FileReader>();

            services.AddScoped<IDisasterCardRepository, JsonDisasterCardRepository>();
            return services;
        }
    }
}
