using ThunderbirdsBoardGameEngine.GameState.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;

namespace ThunderbirdsBoardGameEngine.GameState.WireMock
{
    /// <summary>
    /// Provides extension methods for WireMockHost to create GameStub instances for configuring game-related HTTP
    /// mappings.
    /// </summary>
    /// <remarks>Intended for test fixture setup and fluent stubbing of game state endpoints; GameStub
    /// instances are initialized with the host's WireMock server.</remarks>
    public static class GameStateWireMockExtensions
    {
        /// <summary>
        /// Creates a GameStub that uses the specified host's WireMockServer.
        /// </summary>
        /// <param name="host">The WireMockHost whose WireMockServer is used to create the GameStub.</param>
        /// <returns>A GameStub configured to use the specified host's WireMockServer.</returns>
        public static GameStub GameStub(this WireMockHost host)
        {
            return new GameStub(host.WireMockServer);
        }
    }
}
