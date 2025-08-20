using ThunderbirdsBoardGameEngine.GameData.Api.WireMock.Stubs.V1;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.GameData.Api.WireMock
{
    public sealed class WireMockHost : IAsyncDisposable
    {
        private readonly WireMockServer _wireMockServer;

        public WireMockServer WireMockServer => _wireMockServer;

        public DisasterCardStub DisasterCardStub { get; }

        public string? Url => _wireMockServer.Urls[0];

        public WireMockHost()
        {
            _wireMockServer = WireMockServer.Start(0);
            DisasterCardStub = new DisasterCardStub(_wireMockServer);
        }

        public void Reset()
        {
            _wireMockServer.Reset();
        }

        public async ValueTask DisposeAsync()
        {
            _wireMockServer.Stop();
            _wireMockServer.Dispose();
            await Task.CompletedTask;
        }
    }
}
