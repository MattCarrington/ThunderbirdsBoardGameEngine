using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Entities
{
    public class CharacterDefinitionTests
    {
        [Fact]
        public void Constructor_WhenValidData_SetsPropertiesCorrectly()
        {
            // Arrange
            var rescueBonus = new CharacterRescueBonus(RescueType.Land, 2);
            
            // Act
            var result = new CharacterDefinition(Character.Gordon, rescueBonus);

            // Assert
            Assert.Equal(Character.Gordon, result.Key);
            Assert.NotNull(result.RescueBonus);
            Assert.Equal(rescueBonus.RescueType, result.RescueBonus.RescueType);
            Assert.Equal(rescueBonus.BonusValue, result.RescueBonus.BonusValue);
        }

        [Fact]
        public void Constructor_WhenCharacterHasNoRescueBonus_AllowsNull()
        {
            // Act
            var result = new CharacterDefinition(Character.LadyPenelope, null);

            // Assert
            Assert.Equal(Character.LadyPenelope, result.Key);
            Assert.Null(result.RescueBonus);
        }

        [Fact]
        public void Constructor_WhenLadyPenelopeWithRescueBonus_ThrowsArgumentException()
        {
            // Arrange
            var rescueBonus = new CharacterRescueBonus(RescueType.Sea, 3);

            // Act & Assert
            Assert.Throws<ArgumentException>(() => new CharacterDefinition(Character.LadyPenelope, rescueBonus));
        }
    }
}
