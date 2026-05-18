using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Deserializers;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Loaders;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Providers;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime
{
    /// <summary>
    /// Extension methods for registering reference data services.
    /// </summary>
    public static class ServiceCollectionExtensions
    {
        /// <summary>
        /// Adds reference data services and their dependencies to the specified service collection.
        /// </summary>
        /// <remarks>This method registers singleton implementations for snapshot providers,
        /// deserializers, loaders, and various definition catalogs required for reference data functionality. It is
        /// intended to be called during application startup as part of dependency injection configuration.</remarks>
        /// <param name="services">The service collection to which the reference data services will be added. Cannot be null.</param>
        /// <returns>The same instance of <see cref="IServiceCollection"/> with reference data services registered.</returns>
        public static IServiceCollection AddReferenceData(this IServiceCollection services)
        {
            services.AddLogging();

            services.AddSingleton<ISnapshotProvider, EmbeddedResourceSnapshotProvider>();
            services.AddSingleton<ISnapshotDeserializer, JsonSnapshotDeserializer>();
            services.AddSingleton<SnapshotLoader>();

            services.AddSingleton(sp =>
                sp.GetRequiredService<SnapshotLoader>()
                    .LoadAsync()
                    .ConfigureAwait(false)
                    .GetAwaiter()
                    .GetResult());

            services.AddSingleton<IDisasterDefinitionCatalog, DisasterDefinitionCatalog>();
            services.AddSingleton<ICharacterDefinitionCatalog, CharacterDefinitionCatalog>();
            services.AddSingleton<ILocationDefinitionCatalog, LocationDefinitionCatalog>();
            services.AddSingleton<IDisasterBonusKeyDefinitionCatalog, DisasterBonusKeyDefinitionCatalog>();

            return services;
        }
    }
}
