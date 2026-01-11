using ThunderbirdsBoardGameEngine.Rules.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock
{
    /// <summary>
    /// Provides extension methods for configuring rescue stubs on a WireMock host.
    /// </summary>
    public static class RulesWireMockExtensions
    {
        /// <summary>
        /// Creates a new instance of the RescueStub class for the specified WireMockHost.
        /// </summary>
        /// <param name="host">The WireMockHost to associate with the RescueStub instance. Cannot be null.</param>
        /// <returns>A RescueStub instance associated with the specified WireMockHost.</returns>
        public static RescueStub RescueStub(this WireMockHost host)
        {
            return new RescueStub(host.WireMockServer);
        }
    }
}
