using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.FabCards
{
    public class PersonalHoverjetTests
    {
        [Fact]
        public void GetBonusModifier_InputIsLandRescue_ReturnsAppliedModifier()
        {
            // Arrange
            var personalHoverjet = new PersonalHoverjet();

            var input = new RescueCalculationInput([], RescueType.Land);

            // Act
            var result = personalHoverjet.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single(result);
            Assert.Equal("personal-hoverjet", bonus.Key);
            Assert.Equal(3, bonus.Value);
            Assert.Equal(SourceType.FabCard, bonus.SourceType);
        }

        [Theory]
        [InlineData(RescueType.Air)]
        [InlineData(RescueType.Sea)]
        [InlineData(RescueType.Space)]
        public void GetBonusModifier_InputIsNotLandRescue_ReturnsNoAppliedModifier(RescueType rescueType)
        {
            // Arrange
            var personalHoverjet = new PersonalHoverjet();

            var input = new RescueCalculationInput(Array.Empty<DisasterBonusKey>(), rescueType);

            // Act
            var result = personalHoverjet.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }
    }
}
