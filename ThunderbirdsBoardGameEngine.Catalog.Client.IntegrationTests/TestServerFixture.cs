using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Extensions;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.IntegrationTests
{
    public class TestServerFixture
    {
        public IDisasterCardsClient Client { get; }

        public TestServerFixture()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                { "CatalogClient:BaseAddress", "http://localhost:8080" }
                })
                .Build();

            var services = new ServiceCollection();
            services.AddCatalogClients(config);

            var provider = services.BuildServiceProvider();
            Client = provider.GetRequiredService<IDisasterCardsClient>();
        }
    }
}
