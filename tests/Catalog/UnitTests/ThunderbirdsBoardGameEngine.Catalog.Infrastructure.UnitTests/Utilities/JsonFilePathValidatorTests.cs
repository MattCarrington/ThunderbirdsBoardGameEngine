using System.IO.Abstractions.TestingHelpers;
using System.Runtime.InteropServices;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Utilities
{
    public class JsonFilePathValidatorTests
    {
        private static bool IsWindows =>
            RuntimeInformation.IsOSPlatform(OSPlatform.Windows);

        private const string Key = "Catalog:Json:FilePath";

        [SkippableFact]
        public void Validate_ValidPath_UnixOnly_ReturnsSuccess()
        {
            Skip.If(IsWindows, "Unix-specific absolute path format");

            // Arrange
            var fileSystem = new MockFileSystem();

            var path = "/var/data/data.json";

            fileSystem.AddFile(path, new MockFileData("{}"));

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, path, fileSystem);

            // Assert
            Assert.Empty(result);
        }

        [SkippableTheory]
        [InlineData("C:/path/to/data.json")]
        [InlineData("C:\\path\\to\\data.json")]
        [InlineData("C:/path/to/data.JSON")]
        [InlineData("C:/path/to/data.Json")]
        [InlineData("\\\\server\\share\\cards.json")]
        public void Validate_ValidPath_WindowsOnly_ReturnsSuccess(string path)
        {
            Skip.IfNot(IsWindows, "Windows-specific absolute path formats");

            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(path, new MockFileData("{}"));

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, path, fileSystem);

            // Assert
            Assert.Empty(result);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void Validate_WhenNullOrWhitespaceFilePath_ReturnsFailure(string? path)
        {
            // Arrange

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, path, new MockFileSystem());

            // Assert
            AssertFailureContains(result, "Catalog:Json:FilePath is required.");
        }

        [Fact]
        public void Validate_WhenNotAbsolute_ReturnsFailure()
        {
            // Arrange
            var path = "relative/path/to/data.json";

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, path, new MockFileSystem());

            // Assert
            AssertFailureContains(result, $"Catalog:Json:FilePath must be a fully qualified absolute path after normalisation. Path {path}");
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

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, directory, fileSystem);

            // Assert
            AssertFailureContains(result, "Catalog:Json:FilePath must point to a file, not a directory.");
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

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, missing, fileSystem);

            // Assert
            AssertFailureContains(result, $"Catalog:Json:FilePath must point to an existing file. File not found at {missing}");
        }

        [SkippableTheory]
        [InlineData("C:/path/to/data.txt")]
        [InlineData("C:/path/to/data.doc")]
        [InlineData("C:/path/to/data.docx")]
        [InlineData("C:/path/to/data.json.txt")]
        public void Validate_WhenNotJsonExtension_WindowsOnly_ReturnsFailure(string path)
        {
            Skip.IfNot(IsWindows, "Windows-specific absolute path formats");

            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(path, new MockFileData("Some content"));

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, path, fileSystem);

            // Assert
            AssertFailureContains(result, "Catalog:Json:FilePath must point to a .json extension.");
        }

        [SkippableTheory]
        [InlineData("/var/data/data.txt")]
        [InlineData("/var/data/data.doc")]
        [InlineData("/var/data/data.docx")]
        [InlineData("/var/data/data.json.txt")]
        public void Validate_WhenNotJsonExtension_UnixOnly_ReturnsFailure(string filePath)
        {
            Skip.If(IsWindows, "Unix-specific absolute path formats");

            // Arrange
            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(filePath, new MockFileData("Some content"));

            // Act
            var result = JsonFilePathValidator.ValidateJsonFilePath(Key, filePath, fileSystem);

            // Assert
            AssertFailureContains(result, "Catalog:Json:FilePath must point to a .json extension.");
        }

        private static void AssertFailureContains(IReadOnlyList<string> result, string expectedMessage)
        {
            Assert.NotNull(result);
            Assert.Contains(expectedMessage, result);
        }
    }
}
