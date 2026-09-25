using ThunderbirdsBoardGameEngine.TestUtils.WireMock.xUnit.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.IntegrationTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}
