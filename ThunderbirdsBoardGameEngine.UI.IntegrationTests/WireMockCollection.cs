using ThunderbirdsBoardGameEngine.TestUtils.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}
