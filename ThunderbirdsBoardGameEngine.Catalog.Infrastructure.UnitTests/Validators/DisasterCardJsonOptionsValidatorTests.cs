using Microsoft.Extensions.Options;
using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Validators
{
    public class DisasterCardJsonOptionsValidatorTests
    {
        private static bool IsWindows =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        [SkippableFact]
        public void Validate_ValidPath_UnixOnly_ReturnsSuccess()
        {
            Skip.If(IsWindows, "Unix-specific absolute path format");

            // Arrange
            var fileSystem = new MockFileSystem();

            var path = "/var/data/disastercards.json";

            fileSystem.AddFile(path, new MockFileData("{}"));

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, new DisasterCardJsonOptions { FilePath = path });

            // Assert
            Assert.True(result.Succeeded);
        }

        [SkippableTheory]
        [InlineData("C:/path/to/disastercards.json")]
        [InlineData("C:\\path\\to\\disastercards.json")]
        [InlineData("C:/path/to/disastercards.JSON")]
        [InlineData("C:/path/to/disastercards.Json")]
        [InlineData("\\\\server\\share\\cards.json")]
        public void Validate_ValidPath_WindowsOnly_ReturnsSuccess(string filepath)
        {
            Skip.IfNot(IsWindows, "Windows-specific absolute path formats");

            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(filepath, new MockFileData("{}"));

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, new DisasterCardJsonOptions { FilePath = filepath });

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
            AssertFailureContains(result, "DisasterCardJsonOptions is required.");
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Validate_WhenNullOrWhitespaceFilePath_ReturnsFailure(string? filePath)
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

            var directory = IsWindows 
                ? "C:/path/to/directory/"
                : "/path/to/directory/";

            fileSystem.AddDirectory(directory);

            var options = new DisasterCardJsonOptions
            {
                FilePath = directory
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

            var directory = IsWindows 
                ? "C:/path/to/"
                : "/path/to/";

            var missing = $"{directory}nonexistentfile.json";

            fileSystem.AddDirectory(directory);

            var options = new DisasterCardJsonOptions
            {
                FilePath = missing
            };

            var validator = CreateValidator(fileSystem);

            // Act
            var result = validator.Validate(null, options);

            // Assert
            AssertFailureContains(result, $"Catalog:DisasterCards:Json:FilePath must point to an existing file. File not found at {options.FilePath}");
        }

        [SkippableTheory]
        [InlineData("C:/path/to/disastercards.txt")]
        [InlineData("C:/path/to/disastercards.doc")]
        [InlineData("C:/path/to/disastercards.docx")]
        [InlineData("C:/path/to/disastercards.json.txt")]
        public void Validate_WhenNotJsonExtension_WindowsOnly_ReturnsFailure(string filePath)
        {
            Skip.IfNot(IsWindows, "Windows-specific absolute path formats");

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

        [SkippableTheory]
        [InlineData("/var/data/disastercards.txt")]
        [InlineData("/var/data/disastercards.doc")]
        [InlineData("/var/data/disastercards.docx")]
        [InlineData("/var/data/disastercards.json.txt")]
        public void Validate_WhenNotJsonExtension_UnixOnly_ReturnsFailure(string filePath)
        {
            Skip.If(IsWindows, "Unix-specific absolute path formats");

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
