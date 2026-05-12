using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.UnitTests.Rescue
{
    public class CharacterContributionTests
    {
        [Fact]
        public void ApplyRescueModifier_WhenCharacterRescueTypeDoesNotMatchInput_ReturnsEmptyCollection()
        {
            // Arrange
            var characterContribution = new CharacterContribution(new CharacterCode("Gordon"), new CharacterRescueBonusContribution(RescueType.Sea, 3));

            var input = new RescueCalculationInput(
                presentDisasterBonusKeys: [],
                rescueType: RescueType.Land
            );

            // Act
            var result = characterContribution.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public void ApplyRescueModifier_WhenCharacterRescueTypeMatchesInput_ReturnsBonusModifier()
        {
            // Arrange
            var characterContribution = new CharacterContribution(new CharacterCode("Virgil"), new CharacterRescueBonusContribution(RescueType.Land, 2));

            var input = new RescueCalculationInput(
                presentDisasterBonusKeys: [],
                rescueType: RescueType.Land
            );
            // Act
            var result = characterContribution.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single(result);
            Assert.Equal("Virgil", bonus.Key);
            Assert.Equal(2, bonus.Value);
            Assert.Equal(SourceType.CharacterAbility, bonus.SourceType);
        }

        [Fact]
        public void ApplyRescueModifier_WhenNoRescueBonusContribution_ReturnsEmptyCollection()
        {
            // Arrange
            var characterContribution = new CharacterContribution(new CharacterCode("LadyPenelope"), null);

            var input = new RescueCalculationInput(
                presentDisasterBonusKeys: [],
                rescueType: RescueType.Air
            );

            // Act
            var result = characterContribution.ApplyRescueModifier(input);

            // Assert
            Assert.Empty(result);
        }
    }
}
