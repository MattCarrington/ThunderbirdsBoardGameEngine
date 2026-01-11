using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Fixtures;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.ComponentTests
{
    [CollectionDefinition("WireMock")]
    public class WireMockCollection : ICollectionFixture<WireMockFixture>
    {
    }
}
