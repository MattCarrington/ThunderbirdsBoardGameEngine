using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures
{
    /// <summary>
    /// xUnit fixture that manages the lifecycle of a <see cref="WireMockHost"/>.
    /// </summary>
    public class WireMockFixture : IAsyncLifetime
    {
        /// <summary>
        /// Gets the initialised WireMock host for use in integration tests.
        /// </summary>
        public WireMockHost Host { get; private set; } = null!;

        /// <inheritdoc />
        public async Task InitializeAsync()
        {
            Host = new WireMockHost();
            await Task.CompletedTask;
        }

        /// <inheritdoc />
        public async Task DisposeAsync()
        {
            await Host.DisposeAsync();
        }
    }
}
