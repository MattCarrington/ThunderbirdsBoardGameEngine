using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddOptions<DisasterCardJsonOptions>()
                .Bind(configuration.GetSection("Catalog:DisasterCards:Json"))
                .ValidateOnStart(); // ← run all validators during startup

            services.AddSingleton<IPostConfigureOptions<DisasterCardJsonOptions>, DisasterCardJsonPostConfigure>();
            services.AddSingleton<IValidateOptions<DisasterCardJsonOptions>, DisasterCardJsonOptionsValidator>();
            services.AddSingleton<IFileReader, FileReader>();

            services.AddScoped<IDisasterCardRepository, DisasterCardJsonRepository>();
            return services;
        }
    }
}
