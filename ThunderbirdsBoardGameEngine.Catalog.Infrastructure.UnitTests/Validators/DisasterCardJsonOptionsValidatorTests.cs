using Microsoft.Extensions.Options;
using System.IO.Abstractions.TestingHelpers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Validators
{
    public class DisasterCardJsonOptionsValidatorTests
    {
        [Theory]
        [InlineData("C:/path/to/disastercards.json")]
        [InlineData("C:\\path\\to\\disastercards.json")]
        [InlineData("C:/path/to/disastercards.JSON")]
        [InlineData("C:/path/to/disastercards.Json")]
        [InlineData("\\\\server\\share\\cards.json")]   // UNC absolute path
        public void Validate_ValidPath_ReturnsSuccess(string filepath)
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(filepath, new MockFileData("{}"));

            var options = new DisasterCardJsonOptions
            {
                FilePath = filepath
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
            var result = validator.Validate(null, null);

            // Assert
            AssertFailureContains(result, "DisasterCardJsonOptions is required.");
        }

        [Fact]
        public void Validate_EmptyFilePath_ReturnsFailure()
        {
            // Arrange
            var options = new DisasterCardJsonOptions
            {
                FilePath = string.Empty
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "Catalog:DisasterCards:Json:FilePath is required.");
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Validate_WhenWhitespaceFilePath_ReturnsFailure(string filePath)
        {
            // Arrange
            var options = new DisasterCardJsonOptions
            {
                FilePath = filePath
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "Catalog:DisasterCards:Json:FilePath is required.");
        }

        [Fact]
        public void Validate_WhenNotAbsolute_ReturnsFailure()
        {
            // Arrange
            var options = new DisasterCardJsonOptions
            {
                FilePath = "relative/path/to/disastercards.json"
            };

            var validator = CreateValidator();

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, $"Catalog:DisasterCards:Json:FilePath must be a fully qualified absolute path after normalisation. Path {options.FilePath}");
        }

        [Fact]
        public void Validate_WhenDirectoryPath_ReturnsFailure()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("C:/path/to/directory/");

            var options = new DisasterCardJsonOptions
            {
                FilePath = "C:/path/to/directory/"
            };

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "Catalog:DisasterCards:Json:FilePath must point to a file, not a directory.");
        }

        [Fact]
        public void Validate_WhenFileDoesNotExist_ReturnsFailure()
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddDirectory("C:/path/to/");

            var options = new DisasterCardJsonOptions
            {
                FilePath = "C:/path/to/nonexistentfile.json"
            };

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, $"Catalog:DisasterCards:Json:FilePath must point to an existing file. File not found at {options.FilePath}");
        }

        [Theory]
        [InlineData("C:/path/to/disastercards.txt")]
        [InlineData("C:/path/to/disastercards.doc")]
        [InlineData("C:/path/to/disastercards.docx")]
        [InlineData("C:/path/to/disastercards.json.txt")]
        public void Validate_WhenNotJsonExtension_ReturnsFailure(string filePath)
        {
            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(filePath, new MockFileData("Some content"));

            var options = new DisasterCardJsonOptions
            {
                FilePath = filePath
            };

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, "Catalog:DisasterCards:Json:FilePath must point to a .json extension.");
        }

        private static DisasterCardJsonOptionsValidator CreateValidator()
        {
            var fileSystem = new MockFileSystem();
            return new DisasterCardJsonOptionsValidator(fileSystem);
        }

        private static DisasterCardJsonOptionsValidator CreateValidator(MockFileSystem fileSystem)
        {
            return new DisasterCardJsonOptionsValidator(fileSystem);
        }

        private static void AssertFailureContains(ValidateOptionsResult result, string expectedMessage)
        {
            Assert.False(result.Succeeded);
            Assert.NotNull(result.Failures);
            Assert.Contains(expectedMessage, result.Failures);
        }
    }
}
