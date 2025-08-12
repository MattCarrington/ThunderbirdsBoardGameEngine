using ThunderbirdsBoardGameEngine.GameData.Api.WireMock.Stubs.V1;
using WireMock.Server;

namespace ThunderbirdsBoardGameEngine.GameData.Api.WireMock
{
    internal class ThunderbirdsGameDataApiWireMockServer : IAsyncDisposable
    {
        private readonly WireMockServer _wireMockServer;

        public DisasterCardStub DisasterCardStub { get; }

        public string Url => _wireMockServer.Urls.FirstOrDefault();

        public ThunderbirdsGameDataApiWireMockServer(int? port = null)
        {
            _wireMockServer = WireMockServer.Start(port ?? 5000);
            DisasterCardStub = new DisasterCardStub(_wireMockServer);
        }

        public async ValueTask DisposeAsync()
        {
            _wireMockServer.Stop();
            _wireMockServer.Dispose();
            await Task.CompletedTask;
        }
    }
}
