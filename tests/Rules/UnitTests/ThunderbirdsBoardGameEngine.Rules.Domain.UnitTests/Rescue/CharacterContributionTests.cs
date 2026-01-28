using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
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
            var characterContribution = new CharacterContribution
            {
                Key = CharacterCode.Gordon,
                RescueBonusContribution = new CharacterRescueBonusContribution
                {
                    RescueType = RescueType.Sea,
                    BonusValue = 3
                }
            };            

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
            var characterContribution = new CharacterContribution
            {
                Key = CharacterCode.Virgil,
                RescueBonusContribution = new CharacterRescueBonusContribution
                {
                    RescueType = RescueType.Land,
                    BonusValue = 2
                }
            };

            var input = new RescueCalculationInput(
                presentDisasterBonusKeys: [],
                rescueType: RescueType.Land
            );
            // Act
            var result = characterContribution.ApplyRescueModifier(input);

            // Assert
            var bonus = Assert.Single<AppliedRescueModifier>(result);
            Assert.Equal(CharacterCode.Virgil.ToString(), bonus.Key);
            Assert.Equal(2, bonus.Value);
            Assert.Equal("character", bonus.SourceType);
        }
    }
}
