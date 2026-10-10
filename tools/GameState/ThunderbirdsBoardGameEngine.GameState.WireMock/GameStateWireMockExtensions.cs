using ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;

namespace ThunderbirdsBoardGameEngine.GameState.WireMock
{
    public static class GameStateWireMockExtensions
    {
        public static GameStub GameStub(this WireMockHost host)
        {
            return new GameStub(host.WireMockServer);
        }
    }
}
