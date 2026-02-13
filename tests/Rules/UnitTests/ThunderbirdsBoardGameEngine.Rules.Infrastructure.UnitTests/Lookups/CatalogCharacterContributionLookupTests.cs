using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Lookups
{
    public class CatalogCharacterContributionLookupTests
    {
        [Fact]
        public void GetCharacterContribution_WhenCharacterIsValid_ReturnsCharacterContribution()
        {
            // Arrange
            var character = new CharacterDefinition(Character.Gordon, new(RescueType.Land, 2));

            var lookup = CreateLookup(character);

            // Act
            var result = lookup.GetCharacterContribution(CharacterCode.Gordon);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CharacterCode.Gordon, result.Key);
            Assert.NotNull(result.RescueBonusContribution);
            Assert.Equal(RescueType.Land, result.RescueBonusContribution.RescueType);
            Assert.Equal(2, result.RescueBonusContribution.BonusValue);
        }

        [Fact]
        public void GetCharacterContribution_WhenCharacterHasNoRescueBonus_ReturnsCharacterContributionWithNullRescueBonus()
        {
            // Arrange
            var character = new CharacterDefinition(Character.LadyPenelope, null);

            var lookup = CreateLookup(character);

            // Act
            var result = lookup.GetCharacterContribution(CharacterCode.LadyPenelope);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(CharacterCode.LadyPenelope, result.Key);
            Assert.Null(result.RescueBonusContribution);
        }

        private static CatalogCharacterContributionLookup CreateLookup(CharacterDefinition characterDefinition)
        {
            var source = Substitute.For<ICharacterDefinitionReferenceSource>();
            source.GetCharacterDefinition(Arg.Any<Character>()).Returns(characterDefinition);

            return new CatalogCharacterContributionLookup(source);
        }
    }
}