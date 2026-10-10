using ThunderbirdsBoardGameEngine.TestUtils.WireMock.xUnit.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameState.WireMock.ComponentTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}