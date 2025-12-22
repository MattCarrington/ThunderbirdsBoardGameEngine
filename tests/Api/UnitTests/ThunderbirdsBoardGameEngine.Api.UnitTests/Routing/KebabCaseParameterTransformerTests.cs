using ThunderbirdsBoardGameEngine.Api.Routing;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Routing
{
    public class KebabCaseParameterTransformerTests
    {
        private readonly KebabCaseParameterTransformer _transformer = new();

        [Fact]
        public void TransformOutbound_GivenNull_ReturnsNull()
        {
            // Arrange

            // Act
            var result = _transformer.TransformOutbound(null);

            // Assert
            Assert.Null(result);
        }

        [Theory]
        [InlineData("DisasterCards", "disaster-cards")]     // PascalCase
        [InlineData("disasterCards", "disaster-cards")]     // camelCase
        [InlineData("UserID", "user-id")]                   // lower→UPPER boundary only
        [InlineData("APIResponse", "apiresponse")]          // no split before 'Response'
        [InlineData("IPAddressV4", "ipaddress-v4")]         // digits preserved
        [InlineData("already-kebab", "already-kebab")]      // idempotent
        [InlineData("snake_case", "snake_case")]            // non-matching chars untouched
        public void TransformOutbound_ConvertsAsExpected(string input, string expected)
        {
            // Arrange

            // Act
            var result = _transformer.TransformOutbound(input);

            // Assert
            Assert.Equal(expected, result);
        }
    }
}
