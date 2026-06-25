using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Domain
{
    public class ReferenceDisasterBonusTests
    {
        private static DisasterBonusKey ValidKey => new("bonus-key");

        private static int ValidValue => 1;

        private static LocationCode? ValidLocation => new("location");

        [Fact]
        public void Constructor_WhenAllInputsValid_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceDisasterBonus(
                key: ValidKey,
                value: ValidValue,
                location: ValidLocation
            );

            // Assert
            Assert.Equal(ValidKey, result.Key);
            Assert.Equal(ValidValue, result.Value);
            Assert.Equal(ValidLocation, result.Location);
        }

        [Fact]
        public void Constructor_WhenLocationNull_CreatesInstance()
        {
            // Arrange

            // Act
            var result = new ReferenceDisasterBonus(
                key: ValidKey,
                value: ValidValue,
                location: null
            );

            // Assert
            Assert.Equal(ValidKey, result.Key);
            Assert.Equal(ValidValue, result.Value);
            Assert.Null(result.Location);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Constructor_WhenValueNotPositive_ThrowsArgumentOutOfRangeException(int value)
        {
            // Arrange

            // Act & Assert
            Assert.Throws<ArgumentOutOfRangeException>(() => new ReferenceDisasterBonus(
                key: ValidKey,
                value: value,
                location: ValidLocation
            ));
        }
    }
}