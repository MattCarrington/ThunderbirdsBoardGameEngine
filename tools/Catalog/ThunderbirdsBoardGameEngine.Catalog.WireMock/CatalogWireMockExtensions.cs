using ThunderbirdsBoardGameEngine.Catalog.WireMock.Stubs.V1;
using ThunderbirdsBoardGameEngine.WireMock.Hosting;

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

        /// <summary>
        /// Creates a new instance of the CharactersStub API for configuring character-related HTTP stubs on the
        /// specified WireMock host.
        /// </summary>
        /// <param name="host">The WireMock host to which the character stubs will be attached. Cannot be null.</param>
        /// <returns>A CharactersStub instance that enables setup and management of character-related HTTP stubs for the given
        /// WireMock host.</returns>
        public static CharactersStub CharactersStub(this WireMockHost host)
        {
            return new CharactersStub(host.WireMockServer);
        }
    }
}
