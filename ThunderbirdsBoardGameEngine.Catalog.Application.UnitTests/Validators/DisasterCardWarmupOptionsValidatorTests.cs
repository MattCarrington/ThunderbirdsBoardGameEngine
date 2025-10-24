using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Application.Validators;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Validators
{
    public class DisasterCardWarmupOptionsValidatorTests
    {
        [Fact]
        public void Validate_NullOptions_ReturnsFailure()
        {
            // Arrange
            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, null);

            // Assert
            AssertFailureContains(result, "DisasterCardWarmupOptions is required.");
        }

        [Fact]
        public void Validate_WhenEnabledIsFalse_ReturnsSuccess()
        {
            // Arrange
            var options = new DisasterCardWarmupOptions
            {
                Enabled = false,
                Timeout = TimeSpan.Zero // Irrelevant when disabled
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            Assert.True(result.Succeeded);
        }
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(int.MinValue)]
        public void Validate_WhenTimeoutIsNegative_ReturnsFailure(int seconds)
        {
            // Arrange
            var options = new DisasterCardWarmupOptions
            {
                Enabled = true,
                Timeout = TimeSpan.FromSeconds(seconds)
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "DisasterCardWarmupOptions.Timeout must be greater than zero.");
        }

        [Theory]
        [InlineData(300001)]
        [InlineData(int.MaxValue)]
        public void Validate_WhenTimeoutTooHigh_ReturnsFailure(int milliseconds)
        {
            // Arrange
            var options = new DisasterCardWarmupOptions
            {
                Enabled = true,
                Timeout = TimeSpan.FromMilliseconds(milliseconds)
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "DisasterCardWarmupOptions.Timeout must not exceed 5 minutes.");
        }

        [Theory]
        [InlineData(1)]
        [InlineData(300000)]
        public void Validate_WhenTimeoutIsValid_ReturnsSuccess(int milliseconds)
        {
            // Arrange
            var options = new DisasterCardWarmupOptions
            {
                Enabled = true,
                Timeout = TimeSpan.FromMilliseconds(milliseconds)
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            Assert.True(result.Succeeded);
        }

        private static DisasterCardWarmupOptionsValidator CreateValidator()
        {
            return new DisasterCardWarmupOptionsValidator();
        }

        private static void AssertFailureContains(ValidateOptionsResult result, string expectedMessage)
        {
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failures);
            Assert.Contains(expectedMessage, result.Failures);
        }
    }
}
