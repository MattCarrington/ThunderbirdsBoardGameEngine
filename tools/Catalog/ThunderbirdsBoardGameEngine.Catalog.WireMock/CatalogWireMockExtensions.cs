using ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock
{
    /// <summary>
    /// Provides extension methods for configuring catalog-related stubs on a WireMock host for testing purposes.
    /// </summary>
    public static class CatalogWireMockExtensions
    {
        /// <summary>
        /// Creates a new instance of the DisasterCardStub for the specified WireMockHost.
        /// </summary>
        /// <param name="host">The WireMockHost to associate with the DisasterCardStub.</param>
        /// <returns>A DisasterCardStub instance configured to use the specified WireMockHost.</returns>
        public static DisasterCardStub DisasterCardStub(this WireMockHost host)
        {
            return new DisasterCardStub(host.WireMockServer);
        }
    }
}
