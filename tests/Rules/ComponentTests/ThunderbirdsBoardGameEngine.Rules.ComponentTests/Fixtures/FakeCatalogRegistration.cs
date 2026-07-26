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
            var edges = EdgesTestData.CreateEdges();

            services.AddSingleton<ILocationDefinitionCatalog>(locations);
            services.AddSingleton<IMapEdgeDefinitionCatalog>(edges);

            return services;
        }
    }
}
