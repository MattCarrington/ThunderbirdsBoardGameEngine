using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Rules.Client.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;

namespace ThunderbirdsBoardGameEngine.Rules.Client.UnitTests.Configuration
{
    public class RulesClientOptionsValidatorTests
    {
        [Theory]
        [InlineData("http://example.com")]
        [InlineData("https://example.com/")]
        [InlineData("http://localhost:5000/")]
        [InlineData("https://localhost/")]
        [InlineData("http://127.0.0.1/")]
        [InlineData("https://127.0.0.1/api")]
        [InlineData("https://subdomain.example.com/")]
        public void Validate_ValidBaseAddress_ReturnsSuccess(string baseAddress)
        {
            // Arrange
            var options = new RulesClientOptions
            {
                BaseAddress = baseAddress
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_WhenOptionsIsNull_ReturnsFailure()
        {
            // Arrange
            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, null!);

            // Assert
            AssertFailureContains(result, "RulesClientOptions is required.");
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Validate_WhenNullOrEmptyBaseAddress_ReturnsFailure(string? baseAddress)
        {
            // Arrange
            var options = new RulesClientOptions
            {
                BaseAddress = baseAddress
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "BaseAddress is required.");
        }

        [Theory]
        [InlineData("example.com")]
        [InlineData("localhost")]           // missing scheme
        [InlineData("/api")]                // relative
        [InlineData("http:/example.com")]   // malformed
        [InlineData("file://localhost/api")]
        [InlineData("ftp://example.com")]
        [InlineData("gopher://example.com")]
        [InlineData("ldap://example.com")]
        [InlineData("mailto://example.com")]
        [InlineData("net.pipe://example.com")]
        [InlineData("net.tcp://example.com")]
        [InlineData("news://news.example.com")]
        [InlineData("nntp://news.example.com")]
        [InlineData("telnet://example.com")]
        [InlineData("uuid://example.com")]
        [InlineData("smtp://mail.example.com")]
        public void Validate_InvalidBaseAddress_ReturnsFailure(string baseAddress)
        {
            // Arrange
            var options = new RulesClientOptions
            {
                BaseAddress = baseAddress
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "BaseAddress must be a valid absolute http(s) URI.");
        }

        [Fact]
        public void Validate_WhenBaseAddressContainsQuery_ReturnsFailure()
        {
            // Arrange
            var options = new RulesClientOptions
            {
                BaseAddress = "https://example.com/api?query=param"
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "BaseAddress must not contain query strings.");
        }

        [Fact]
        public void Validate_WhenBaseAddressContainsFragment_ReturnsFailure()
        {
            // Arrange
            var options = new RulesClientOptions
            {
                BaseAddress = "https://example.com/api#fragment"
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "BaseAddress must not contain a fragment.");
        }

        private static RulesClientOptionsValidator CreateValidator()
        {
            return new RulesClientOptionsValidator();
        }

        private static void AssertFailureContains(ValidateOptionsResult result, string expectedMessage)
        {
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failures);
            Assert.Contains(expectedMessage, result.Failures);
        }
    }
}
