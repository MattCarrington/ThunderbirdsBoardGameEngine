using NSubstitute;
using NSubstitute.ExceptionExtensions;
using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.UnitTests.Lookups
{
    public class CatalogCharacterCatalogLookupTests
    {
        [Fact]
        public void GetCharacterRescueContribution_WhenCharacterIsValid_ReturnsCharacterContribution()
        {
            // Arrange
            var code = new CharacterCode("gordon");

            var character = new ReferenceCharacterDefinition(
                code: code,
                displayName: "Gordon",
                rescueBonus: new ReferenceCharacterRescueBonus(RescueType.Sea, 2)
            );

            var lookup = CreateLookup(character);

            // Act
            var result = lookup.GetCharacterRescueContribution(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Key);
            Assert.NotNull(result.RescueBonusContribution);
            Assert.Equal(RescueType.Sea, result.RescueBonusContribution.RescueType);
            Assert.Equal(2, result.RescueBonusContribution.BonusValue);
        }

        [Fact]
        public void GetCharacterRescueContribution_WhenCharacterHasNoRescueBonus_ReturnsCharacterContributionWithNullRescueBonus()
        {
            // Arrange
            var code = new CharacterCode("lady_penelope");

            var character = new ReferenceCharacterDefinition(code, "Lady Penelope", null);

            var lookup = CreateLookup(character);

            // Act
            var result = lookup.GetCharacterRescueContribution(code);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(code, result.Key);
            Assert.Null(result.RescueBonusContribution);
        }

        [Fact]
        public void GetCharacterRescueContribution_WhenCharacterIsNotFound_ThrowsInvalidRescueCalculationRequestException()
        {
            // Arrange
            var code = new CharacterCode("unknown_character");

            var catalog = Substitute.For<ICharacterDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<CharacterCode>()).Throws(new KeyNotFoundException());

            var lookup = new ReferenceCharacterCatalogLookup(catalog);

            // Act & Assert
            var ex = Assert.Throws<InvalidRescueCalculationRequestException>(() => lookup.GetCharacterRescueContribution(code));
            Assert.Equal("Character", ex.ResourceType);
            Assert.Equal(code.ToString(), ex.Code);
        }

        private static ReferenceCharacterCatalogLookup CreateLookup(ReferenceCharacterDefinition characterDefinition)
        {
            var catalog = Substitute.For<ICharacterDefinitionCatalog>();
            catalog.GetByCode(Arg.Any<CharacterCode>()).Returns(characterDefinition);

            return new ReferenceCharacterCatalogLookup(catalog);
        }
    }
}