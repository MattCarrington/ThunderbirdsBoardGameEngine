using ThunderbirdsBoardGameEngine.TestUtils.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.ComponentTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}
