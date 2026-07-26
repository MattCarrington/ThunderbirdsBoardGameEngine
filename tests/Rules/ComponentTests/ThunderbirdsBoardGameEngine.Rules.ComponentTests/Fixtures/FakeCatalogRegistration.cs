using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.ComponentTests.TestData;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fixtures
{
    public static class FakeCatalogRegistration
    {
        public static IServiceCollection AddFakeCatalogs(this IServiceCollection services)
        {
            var edges = EdgesTestData.CreateEdges();

            services.AddSingleton<IMapEdgeDefinitionCatalog>(edges);
            return services;
        }
    }
}
