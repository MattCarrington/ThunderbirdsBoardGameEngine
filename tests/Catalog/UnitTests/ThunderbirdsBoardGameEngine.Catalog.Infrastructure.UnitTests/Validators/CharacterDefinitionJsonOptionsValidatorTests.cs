using System.IO.Abstractions;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Validators
{
    public class CharacterDefinitionJsonOptionsValidatorTests
    {
        [Fact]
        public void Validate_ValidOptions_ReturnsSuccess()
        {
            // Arrange
            var path = RuntimeInformation.IsOSPlatform(OSPlatform.Windows)
                ? "C:/path/to/disastercards.json"
                : "/path/to/disastercards.json";

            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(path, new MockFileData("{}"));

            var options = new CharacterJsonOptions
            {
                FilePath = path
            };

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, options);

            // Assert
            Assert.True(result.Succeeded);
        }

        [Fact]
        public void Validate_NullOptions_ReturnsFailure()
        {
            // Arrange
            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, null!);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failures);
            Assert.Contains("CharacterDefintionJsonOptions is required.", result.Failures);
        }

        [Fact]
        public void Validate_WhenFilePathMissing_UsesDisasterCardsConfigKey()
        {
            // Arrange
            var options = new CharacterJsonOptions { FilePath = "" };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failures);

            var failureMessage = Assert.Single(result.Failures);
            Assert.Contains("Catalog:Characters:Json:FilePath", failureMessage);
        }

        private static CharacterDefinitionJsonOptionsValidator CreateValidator()
        {
            var fileSystem = new MockFileSystem();
            return CreateValidator(fileSystem);
        }

        private static CharacterDefinitionJsonOptionsValidator CreateValidator(IFileSystem fileSystem)
        {
            return new CharacterDefinitionJsonOptionsValidator(fileSystem);
        }
    }
}
