using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fixtures
{
    public static class FakeCatalogRegistration
    {
        public static IServiceCollection AddFakeCatalogs(this IServiceCollection services)
        {
            var locations = LocationsTestData.CreateLocations();
            var characters = CharactersTestData.CreateCharacterCatalog();
            var thunderbirds = ThunderbirdsTestData.CreateThunderbirds();
            var disasters = DisasterCardsTestData.CreateDisasterCatalog();
            var edges = EdgesTestData.CreateEdges();
            var fabCards = FabCardsTestData.CreateFabCardsCatalog();
            var eventCards = EventCardsTestData.CreateEventCardsCatalog();

            services.AddSingleton<ILocationDefinitionCatalog>(locations);
            services.AddSingleton<ICharacterDefinitionCatalog>(characters);
            services.AddSingleton<IThunderbirdDefinitionCatalog>(thunderbirds);
            services.AddSingleton<IDisasterDefinitionCatalog>(disasters);
            services.AddSingleton<IMapEdgeDefinitionCatalog>(edges);
            services.AddSingleton<IFabCardDefinitionCatalog>(fabCards);
            services.AddSingleton<IEventCardDefinitionCatalog>(eventCards);

            return services;
        }
    }
}
