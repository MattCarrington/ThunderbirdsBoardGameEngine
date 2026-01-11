using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.WireMock.Hosting
{
    /// <summary>
    /// Hosts a WireMock server instance configured with Catalog API stubs.
    /// </summary>
    /// <remarks>
    /// <para>
    /// <see cref="WireMockHost"/> provides a single entry point for standing up
    /// a WireMock server with preconfigured, versioned stubs for integration
    /// and consumer testing.
    /// </para>
    /// <para>
    /// The server is started on a random available port and exposes typed stub
    /// helpers (for example, <see cref="DisasterCardStub"/>) to register common
    /// success and error scenarios.
    /// </para>
    /// </remarks>
    public sealed class WireMockHost : IAsyncDisposable
    {
        private readonly WireMockServer _wireMockServer;

        /// <summary>
        /// Gets the underlying <see cref="WireMockServer"/> instance.
        /// </summary>
        /// <remarks>
        /// Exposed primarily for advanced scenarios that require direct access
        /// to WireMock APIs not covered by the provided stub helpers.
        /// </remarks>
        public WireMockServer WireMockServer => _wireMockServer;

        /// <summary>
        /// Gets the base URL of the running WireMock server.
        /// </summary>
        /// <remarks>
        /// This value can be supplied directly to HTTP clients under test.
        /// </remarks>
        public string? Url => _wireMockServer.Urls[0];

        /// <summary>
        /// Initializes a new instance of the <see cref="WireMockHost"/> class.
        /// </summary>
        /// <remarks>
        /// The WireMock server is started immediately on a random available port,
        /// and all Catalog API stubs are initialized.
        /// </remarks>
        public WireMockHost()
        {
            _wireMockServer = WireMockServer.Start(0);
        }

        /// <summary>
        /// Resets all registered stubs and recorded request logs.
        /// </summary>
        /// <remarks>
        /// Intended for use between test cases to ensure isolation and
        /// repeatability.
        /// </remarks>
        public void Reset()
        {
            _wireMockServer.Reset();
            _wireMockServer.ResetLogEntries();
        }

        /// <summary>
        /// Stops and disposes the underlying WireMock server.
        /// </summary>
        public async ValueTask DisposeAsync()
        {
            _wireMockServer.Stop();
            _wireMockServer.Dispose();
            await Task.CompletedTask;
        }
    }
}
