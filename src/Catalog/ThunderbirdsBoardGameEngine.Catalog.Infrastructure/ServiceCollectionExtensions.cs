using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Decorators;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Deserializers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initializers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure
{
    /// <summary>
    /// Extension methods for registering catalog infrastructure services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Registers the catalog infrastructure services required to load and validate
        /// disaster card data from JSON into an in-memory catalog.
        /// </summary>
        /// <param name="services">The service collection to add services to.</param>
        /// <param name="configuration">
        /// The application configuration, used to bind
        /// <see cref="DisasterCardJsonOptions"/> and JSON serialization options.
        /// </param>
        /// <returns>The same <see cref="IServiceCollection"/> instance, for chaining.</returns>
        /// <remarks>
        /// This method configures:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="DisasterCardJsonOptions"/> bound from <c>Catalog:DisasterCards:Json</c>
        ///       and validated on startup.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       File system abstractions and JSON readers/deserializers for disaster cards.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       An <see cref="IDisasterCardReferenceSource"/> that is initialized eagerly at startup.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       A named <see cref="JsonSerializerOptions"/> instance for catalog serialization.
        ///     </description>
        ///   </item>
        /// </list>
        /// </remarks>
        public static IServiceCollection AddCatalogInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IFileOpener, FileOpener>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IEnvelopeParser, EnvelopeParser>();

            services.AddSingleton<IPostConfigureOptions<DisasterCardJsonOptions>, DisasterCardJsonPostConfigure>();
            services.AddSingleton<IValidateOptions<DisasterCardJsonOptions>, DisasterCardJsonOptionsValidator>();
            services.AddSingleton<IDisasterCardMapper, DisasterCardMapper>();
            services.AddSingleton<IDisasterCardDeserializer, DisasterCardDeserializer>();
            services.AddSingleton<IDisasterCardReader, DisasterCardJsonReader>();
            services.Decorate<IDisasterCardReader, ValidatingDisasterCardReader>();
            services.AddSingleton<IDisasterCardReferenceSource>(sp =>
            {
                var init = new DisasterCardReferenceSourceInitializer(sp.GetRequiredService<IDisasterCardReader>());
                return init.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            });

            services.AddSingleton(sp =>
                (IDisasterCardReferenceSourceProbe)sp.GetRequiredService<IDisasterCardReferenceSource>());

            services.AddOptions<DisasterCardJsonOptions>()
                .Bind(configuration.GetSection("Catalog:DisasterCards:Json"))
                .ValidateOnStart(); // ← run all validators during startup

            services.AddOptions<JsonSerializerOptions>(CatalogJson.Name)
                .Configure(CatalogJson.Configure);

            return services;
        }
    }
}
