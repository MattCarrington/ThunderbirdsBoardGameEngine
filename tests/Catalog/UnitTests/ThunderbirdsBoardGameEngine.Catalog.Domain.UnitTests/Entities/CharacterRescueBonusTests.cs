using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.UnitTests.Entities
{
    public class CharacterRescueBonusTests
    {
        [Fact]
        public void Constructor_WhenCalled_SetsPropertiesCorrectly()
        {
            // Arrange
            var expectedRescueType = RescueType.Land;
            var expectedBonusValue = 5;

            // Act
            var characterRescueBonus = new CharacterRescueBonus(expectedRescueType, expectedBonusValue);

            // Assert
            Assert.Equal(expectedRescueType, characterRescueBonus.RescueType);
            Assert.Equal(expectedBonusValue, characterRescueBonus.BonusValue);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Constructor_WhenBonusValueInvalid_ThrowsArgumentOutOfRangeException(int invalidBonusValue)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new CharacterRescueBonus(RescueType.Air, invalidBonusValue));
        }
    }
}
