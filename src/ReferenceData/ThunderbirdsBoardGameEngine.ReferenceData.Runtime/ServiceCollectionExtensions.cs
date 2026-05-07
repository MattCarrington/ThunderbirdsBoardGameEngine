using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Deserializers;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Loaders;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Providers;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddReferenceData(this IServiceCollection services)
        {
            services.AddSingleton<ISnapshotProvider, EmbeddedResourceSnapshotProvider>();
            services.AddSingleton<ISnapshotDeserializer, JsonSnapshotDeserializer>();
            services.AddSingleton<SnapshotLoader>();

            services.AddSingleton(sp =>
                sp.GetRequiredService<SnapshotLoader>()
                    .LoadAsync()
                    .GetAwaiter()
                    .GetResult());

            services.AddSingleton<IDisasterDefinitionCatalog, DisasterDefinitionCatalog>();
            services.AddSingleton<ICharacterDefinitionCatalog, CharacterDefinitionCatalog>();

            return services;
        }
    }
}
