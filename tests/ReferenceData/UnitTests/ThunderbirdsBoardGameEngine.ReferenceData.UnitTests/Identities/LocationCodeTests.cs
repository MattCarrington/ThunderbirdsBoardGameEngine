using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Identities
{
    public class LocationCodeTests
    {
        [Fact]
        public void Constructor_WhenValidCode_CreatesInstance()
        {
            // Arrange
            var code = "valid-location-code";

            // Act
            var result = new LocationCode(code);

            // Assert
            Assert.Equal(code, result.Value);
            Assert.Equal(code, result.ToString());
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenInvalidLocationValues_ThrowsArgumentException(string code)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new LocationCode(code));
        }
    }
}