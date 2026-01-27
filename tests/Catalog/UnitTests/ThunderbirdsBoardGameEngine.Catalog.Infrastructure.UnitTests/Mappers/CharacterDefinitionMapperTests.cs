using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Mappers
{
    public class CharacterDefinitionMapperTests
    {
        [Theory]
        [InlineData("LadyPenelope")]
        [InlineData("ladypenelope")]
        [InlineData("LADYPENELOPE")]
        [InlineData("ladyPenelope")]
        [InlineData("Ladypenelope")]
        public void Map_CharacterWithNoRescueBonuses_ShouldMapRequiredProperties(string input)
        {
            // Arrange
            var dto = new CharacterCatalogDto
            {
                Key = input,
                RescueBonuses = []
            };

            var mapper = CreateMapper();

            // Act
            CharacterDefinition result = mapper.Map(dto);

            // Assert
            Assert.Equal(Character.LadyPenelope, result.Key);
            Assert.Null(result.RescueBonus);
        }

        [Theory]
        [InlineData("air")]
        [InlineData("Air")]
        [InlineData("AIR")]
        public void Map_CharacterWithRescueBonus_ShouldMapRescueBonus(string input)
        {
            // Arrange
            var dto = new CharacterCatalogDto
            {
                Key = "Scott",
                RescueBonuses = new List<CharacterRescueBonusCatalogDto>
                {
                    new() {
                        RescueType = input,
                        BonusValue = 2
                    }
                }
            };

            var mapper = CreateMapper();

            // Act
            CharacterDefinition result = mapper.Map(dto);

            // Assert
            Assert.Equal(Character.Scott, result.Key);
            Assert.NotNull(result.RescueBonus);
            Assert.Equal(RescueType.Air, result.RescueBonus!.RescueType);
            Assert.Equal(2, result.RescueBonus.BonusValue);
        }

        [Fact]
        public void Map_CharacterWithMultipleRescueBonuses_ShouldThrowArgumentException()
        {
            // Arrange
            var dto = new CharacterCatalogDto
            {
                Key = "Virgil",
                RescueBonuses =
                [
                    new() {
                        RescueType = "Land",
                        BonusValue = 1
                    },
                    new() {
                        RescueType = "Sea",
                        BonusValue = 2
                    }
                ]
            };

            var mapper = CreateMapper();

            // Act & Assert
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => mapper.Map(dto));
            Assert.Equal(CharacterDefinitionErrorCode.InvalidRescueBonusCount, exception.ErrorCode);
            Assert.Equal("Character 'Virgil' has more than one rescue bonus.", exception.Message);
        }

        [Fact]
        public void Map_CharacterDtoNull_ShouldThrowArgumentNullException()
        {
            // Arrange
            var mapper = CreateMapper();

            // Act & Assert
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => mapper.Map(null));
            Assert.Equal(CharacterDefinitionErrorCode.NullEntry, exception.ErrorCode);
        }

        [Fact]
        public void Map_CharacterDtoInvalid_WrapsUnknownException()
        {
            // Arrange
            var invalidCharacter = "brains";

            var dto = new CharacterCatalogDto
            {
                Key = invalidCharacter,
                RescueBonuses = []
            };

            var mapper = CreateMapper();

            // Act
            var exception = Assert.Throws<CharacterDefinitionValidationException>(() => mapper.Map(dto));
            Assert.Equal(CharacterDefinitionErrorCode.Unknown, exception.ErrorCode);
            var inner = Assert.IsType<ArgumentException>(exception.InnerException);
            Assert.Contains($"Invalid enum value: {invalidCharacter}", inner.Message);
        }

        private static CharacterDefinitionMapper CreateMapper()
        {
            return new CharacterDefinitionMapper();
        }
    }
}
