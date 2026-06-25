using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.UnitTests.Identities
{
    public class ThunderbirdCodeTests
    {
        [Fact]
        public void Constructor_WhenValidCode_CreatesInstance()
        {
            // Arrange
            var code = "valid-thunderbird-code";

            // Act
            var result = new ThunderbirdCode(code);

            // Assert
            Assert.Equal(code, result.Value);
            Assert.Equal(code, result.ToString());
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Constructor_WhenInvalidCardValues_ThrowsArgumentException(string? code)
        {
            // Arrange

            // Act & Assert
            Assert.ThrowsAny<ArgumentException>(() => new ThunderbirdCode(code));
        }
    }
}
