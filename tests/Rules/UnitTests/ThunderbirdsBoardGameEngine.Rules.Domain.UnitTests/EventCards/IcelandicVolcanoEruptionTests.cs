using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.Rules.Domain.EventCards;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.EventCards
{
    public class IcelandicVolcanoEruptionTests
    {
        [Theory]
        [InlineData("europe", "north-atlantic")]
        [InlineData("north-atlantic", "europe")]
        [InlineData("africa", "north-atlantic")]
        [InlineData("north-atlantic", "africa")]
        [InlineData("africa", "south-atlantic")]
        [InlineData("south-atlantic", "africa")]
        public void ApplyMovementModifier_BlocksExpectedUndirectedEdge(string location1, string location2)
        {
            // Arrange
            var card = new IcelandicVolcanoEruption();
            var edge = new ReferenceMapEdgeDefinition(new LocationCode(location1), new LocationCode(location2), ReferenceData.Core.Enums.MovementDomain.Earth);

            // Act
            var result = card.ApplyMovementModifier();

            // Assert
            Assert.Equal(KnownEventCardCodes.IcelandicVolcanoEruption, result.Card);
            Assert.Contains(result.BlockedEdges, blockedEdge => blockedEdge.Matches(edge));
        }

        [Fact]
        public void ApplyMovementModifier_ReturnsExactlyThreeBlockedEdges()
        {
            // Arrange

            // Act
            var result = new IcelandicVolcanoEruption().ApplyMovementModifier();

            // Assert
            Assert.Equal(3, result.BlockedEdges.Count);
            Assert.False(string.IsNullOrWhiteSpace(result.Message));
        }
    }
}
