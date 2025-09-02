using ThunderbirdsBoardGameEngine.TestUtils.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.WireMock.ComponentTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}