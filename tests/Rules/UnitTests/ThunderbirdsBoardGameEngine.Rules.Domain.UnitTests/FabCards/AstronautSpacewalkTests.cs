using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.FabCards
{
    public class AstronautSpacewalkTests
    {
        [Fact]
        public void GetBonusModifier_InputIsSpaceRescue_ReturnsAppliedModifier()
        {
            // Arrange
            var astronautSpacewalk = new AstronautSpacewalk();

            var input = new RescueCalculationInput([], RescueType.Space);

            // Act
            var result = astronautSpacewalk.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single(result);
            Assert.Equal("astronaut-spacewalk", bonus.Key);
            Assert.Equal(3, bonus.Value);
            Assert.Equal(SourceType.FabCard, bonus.SourceType);
        }

        [Theory]
        [InlineData(RescueType.Air)]
        [InlineData(RescueType.Land)]
        [InlineData(RescueType.Sea)]
        public void GetBonusModifier_InputIsNotSpaceRescue_ReturnsNoAppliedModifier(RescueType rescueType)
        {
            // Arrange
            var astronautSpacewalk = new AstronautSpacewalk();

            var input = new RescueCalculationInput(Array.Empty<DisasterBonusKey>(), rescueType);

            // Act
            var result = astronautSpacewalk.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }
    }
}
