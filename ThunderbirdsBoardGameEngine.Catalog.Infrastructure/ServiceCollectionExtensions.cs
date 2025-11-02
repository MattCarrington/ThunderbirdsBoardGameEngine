using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Decorators;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initialisers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IFileOpener, FileOpener>();
            services.AddSingleton<IFileSystem, FileSystem>();

            services.AddSingleton<IPostConfigureOptions<DisasterCardJsonOptions>, DisasterCardJsonPostConfigure>();
            services.AddSingleton<IValidateOptions<DisasterCardJsonOptions>, DisasterCardJsonOptionsValidator>();
            services.AddSingleton<IDisasterCardMapper, DisasterCardMapper>();
            services.AddSingleton<IDisasterCardReader, DisasterCardJsonReader>();
            services.Decorate<IDisasterCardReader, ValidatingDisasterCardReader>();
            services.AddSingleton<IDisasterCardCatalog>(sp =>
            {
                var init = new DisasterCardCatalogInitializer(sp.GetRequiredService<IDisasterCardReader>());
                return init.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            });

            services.AddSingleton(sp =>
                (IDisasterCardCatalogProbe)sp.GetRequiredService<IDisasterCardCatalog>());

            services.AddOptions<DisasterCardJsonOptions>()
                .Bind(configuration.GetSection("Catalog:DisasterCards:Json"))
                .ValidateOnStart(); // ← run all validators during startup

            services.AddOptions<JsonSerializerOptions>(CatalogJsonDefaults.Name)
                .Configure(CatalogJsonDefaults.Configure);

            return services;
        }
    }
}
