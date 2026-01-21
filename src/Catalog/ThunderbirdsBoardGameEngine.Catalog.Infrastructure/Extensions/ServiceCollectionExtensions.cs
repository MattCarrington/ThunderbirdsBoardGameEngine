using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Decorators;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Decorators;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Deserializers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initializers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PayloadReaders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PostConfigures;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.StreamSources;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Extensions
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
            AddCatalogCore(services);
            AddDisasterCards(services, configuration);
            AddCharacters(services, configuration);

            services.AddOptions<JsonSerializerOptions>(CatalogJson.Name)
                .Configure(CatalogJson.Configure);

            return services;
        }

        private static void AddCatalogCore(IServiceCollection services)
        {
            services.AddSingleton<IFileOpener, FileOpener>();
            services.AddSingleton<IFileSystem, FileSystem>();
            services.AddSingleton<IEnvelopeParser, EnvelopeParser>();
            services.AddSingleton<IJsonStreamValidator, JsonStreamValidator>();
            services.AddSingleton<ICatalogStreamSource, FileSystemCatalogStreamSource>();

            // Generated-catalog support
            services.AddSingleton<IGeneratedContentValidator, GeneratedContentValidator>();

            // Register both manifest types explicitly (needed for Scrutor decoration)
            services.AddScoped<ICatalogPayloadReader<SimpleCatalogManifest>, JsonCatalogPayloadReader<SimpleCatalogManifest>>();
            services.AddScoped<ICatalogPayloadReader<GeneratedCatalogManifest>, JsonCatalogPayloadReader<GeneratedCatalogManifest>>();

            // Generated catalogs get extra validation layer (between Json and ExceptionMapping)
            services.Decorate<ICatalogPayloadReader<GeneratedCatalogManifest>, GeneratedJsonCatalogPayloadReader>();

            // ALL catalogs get exception mapping (outermost boundary)
            services.Decorate<ICatalogPayloadReader<SimpleCatalogManifest>, ExceptionMappingCatalogPayloadReader<SimpleCatalogManifest>>();
            services.Decorate<ICatalogPayloadReader<GeneratedCatalogManifest>, ExceptionMappingCatalogPayloadReader<GeneratedCatalogManifest>>();
        }

        private static void AddDisasterCards(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPostConfigureOptions<DisasterCardJsonOptions>, DisasterCardJsonPostConfigure>();
            services.AddSingleton<IValidateOptions<DisasterCardJsonOptions>, DisasterCardJsonOptionsValidator>();
            services.AddSingleton<IDisasterCardMapper, DisasterCardMapper>();
            services.AddSingleton<IDisasterCardDeserializer, DisasterCardDeserializer>();
            services.AddScoped<IDisasterCardReader, DisasterCardJsonReader>();
            services.Decorate<IDisasterCardReader, ValidatingDisasterCardReader>();
            services.AddSingleton<IDisasterCardReferenceSource>(sp =>
            {
                var scope = sp.CreateScope();

                var reader = scope.ServiceProvider.GetRequiredService<IDisasterCardReader>();

                var init = new DisasterCardReferenceSourceInitializer(reader);
                return init.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            });

            services.AddSingleton(sp =>
                (IDisasterCardReferenceSourceProbe)sp.GetRequiredService<IDisasterCardReferenceSource>());

            services.AddOptions<DisasterCardJsonOptions>()
                .Bind(configuration.GetSection("Catalog:DisasterCards:Json"))
                .ValidateOnStart(); // ← run all validators during startup            
        }

        private static void AddCharacters(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<IPostConfigureOptions<CharacterJsonOptions>, CharacterDefinitionJsonPostConfigure>();
            services.AddSingleton<IValidateOptions<CharacterJsonOptions>, CharacterDefinitionJsonOptionsValidator>();
            services.AddSingleton<ICharacterDeserializer, CharacterDeserializer>();
            services.AddSingleton<ICharacterMapper, CharacterMapper>();
            services.AddScoped<ICharacterReader, CharacterJsonReader>();
            services.Decorate<ICharacterReader, ValidatingCharacterDefinitionReader>();

            services.AddSingleton<ICharacterReferenceSource>(sp =>
            {
                var scope = sp.CreateScope();

                var reader = scope.ServiceProvider.GetRequiredService<ICharacterReader>();

                var init = new CharacterDefinitionReferenceSourceInitializer(reader);
                return init.InitializeAsync(CancellationToken.None).GetAwaiter().GetResult();
            });

            //services.AddSingleton(sp =>
            //    (IDisasterCardReferenceSourceProbe)sp.GetRequiredService<IDisasterCardReferenceSource>());

            services.AddOptions<CharacterJsonOptions>()
                .Bind(configuration.GetSection("Catalog:Characters:Json"))
                .ValidateOnStart(); // ← run all validators during startup
        }
    }
}
