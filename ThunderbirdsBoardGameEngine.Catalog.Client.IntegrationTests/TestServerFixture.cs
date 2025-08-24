using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.IntegrationTests
{
    public class TestServerFixture
    {
        public IDisasterCardClient Client { get; }

        public TestServerFixture()
        {
            var config = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                { "GameDataClient:BaseAddress", "http://localhost:8080" }
                })
                .Build();

            var services = new ServiceCollection();
            services.AddGameDataClients(config);

            var provider = services.BuildServiceProvider();
            Client = provider.GetRequiredService<IDisasterCardClient>();
        }
    }
}
