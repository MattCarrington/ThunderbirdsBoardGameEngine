using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.FabCards
{
    public class UnderwaterSealingUnitTests
    {
        [Fact]
        public void GetBonusModifier_InputIsSeaRescue_ReturnsAppliedModifier()
        {
            // Arrange
            var underwaterSealingUnit = new UnderwaterSealingUnit();

            var input = new RescueCalculationInput(Array.Empty<DisasterBonusKey>(), RescueType.Sea);

            // Act
            var result = underwaterSealingUnit.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single(result);
            Assert.Equal("underwater-sealing-unit", bonus.Key);
            Assert.Equal(3, bonus.Value);
            Assert.Equal(SourceType.FabCard, bonus.SourceType);
        }

        [Theory]
        [InlineData(RescueType.Air)]
        [InlineData(RescueType.Land)]
        [InlineData(RescueType.Space)]
        public void GetBonusModifier_InputIsNotSeaRescue_ReturnsNoAppliedModifier(RescueType rescueType)
        {
            // Arrange
            var underwaterSealingUnit = new UnderwaterSealingUnit();

            var input = new RescueCalculationInput([], rescueType);

            // Act
            var result = underwaterSealingUnit.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }
    }
}
