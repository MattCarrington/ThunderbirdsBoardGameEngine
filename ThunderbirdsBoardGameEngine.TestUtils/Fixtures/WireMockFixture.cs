using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.Fixtures
{
    public class WireMockFixture : IAsyncLifetime
    {
        public WireMockHost Host { get; private set; } = default!;

        public async Task InitializeAsync()
        {
            Host = new WireMockHost();
            await Task.CompletedTask;
        }

        public async Task DisposeAsync()
        {
            await Host.DisposeAsync();
        }        
    }
}
