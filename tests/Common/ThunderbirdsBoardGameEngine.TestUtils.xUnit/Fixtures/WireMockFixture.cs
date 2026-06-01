using ThunderbirdsBoardGameEngine.WireMock.Hosting;
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
        public ValueTask InitializeAsync()
        {
            Host = new WireMockHost();
            return ValueTask.CompletedTask;
        }

        /// <inheritdoc />
        public async ValueTask DisposeAsync()
        {
            await Host.DisposeAsync();
        }
    }
}
