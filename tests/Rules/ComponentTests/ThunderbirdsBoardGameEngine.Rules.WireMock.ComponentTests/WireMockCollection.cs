using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.WireMock.ComponentTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}