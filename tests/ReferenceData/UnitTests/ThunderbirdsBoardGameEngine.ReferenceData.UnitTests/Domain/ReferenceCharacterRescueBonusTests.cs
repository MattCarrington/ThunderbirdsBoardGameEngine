using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceCharacterRescueBonusTests
    {
        public static RescueType ValidRescueType => RescueType.Sea;

        public static int ValidRescueBonusValue => 1;

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceCharacterRescueBonus(
                ValidRescueType,
                ValidRescueBonusValue);

            // Assert
            Assert.Equal(ValidRescueType, result.RescueType);
            Assert.Equal(ValidRescueBonusValue, result.Value);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Constructor_WhenValueNotPositive_ThrowsArgumentOutOfRangeException(int value)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceCharacterRescueBonus(ValidRescueType, value));
        }
    }
}
