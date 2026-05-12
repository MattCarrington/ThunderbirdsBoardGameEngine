using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Identities
{
    public class DisasterBonusKeyTests
    {
        [Fact]
        public void Constructor_WhenValidKey_CreatesInstance()
        {
            // Arrange
            var key = "valid-bonus-key";

            // Act
            var result = new DisasterBonusKey(key);

            // Assert
            Assert.Equal(key, result.Value);
            Assert.Equal(key, result.ToString());
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenInvalidKeyValues_ThrowsArgumentException(string key)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new DisasterBonusKey(key));
        }
    }
}