using System.IO.Abstractions.TestingHelpers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Utilities
{
    public class FileOpenerTests
    {
        [Fact]
        public async Task OpenReadAsync_WithStringEmptyPath_ThrowsArgumentNullException()
        {
            // Arrange
            var fileReader = CreateFileReader();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => fileReader.OpenReadAsync(string.Empty, CancellationToken.None));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task OpenReadAsync_WithNullOrWhitespacePath_ThrowsArgumentNullException(string path)
        {
            // Arrange
            var fileReader = CreateFileReader();

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentException>(() => fileReader.OpenReadAsync(path, CancellationToken.None));
        }

        [Fact]
        public async Task OpenReadAsync_WhenCancelledToken_ReturnsCancelledTaskAsync()
        {
            // Arrange
            using var cancellationToken = new CancellationTokenSource();
            await cancellationToken.CancelAsync();

            var fileReader = CreateFileReader();

            // Act
            var result = fileReader.OpenReadAsync(@"C:\data\file.json", cancellationToken.Token);

            // Assert
            Assert.True(result.IsCanceled);
            await Assert.ThrowsAsync<TaskCanceledException>(() => result);
        }

        [Fact]
        public async Task OpenReadAsync_WhenCalled_ReturnsReadableStreamWithContentAsync()
        {
            // Arrange
            var path = @"C:\data\file.json";

            var fileSystem = new MockFileSystem();
            fileSystem.AddFile(path, new MockFileData("{ \"key\": \"value\" }"));

            var fileReader = CreateFileReader(fileSystem);

            // Act
            using var result = await fileReader.OpenReadAsync(path, CancellationToken.None);

            // Assert
            Assert.True(result.CanRead);

            using var reader = new StreamReader(result);
            var content = await reader.ReadToEndAsync();

            Assert.Equal("{ \"key\": \"value\" }", content);
        }

        [Fact]
        public async Task OpenReadAsync_WhenFileMissing_ThrowsFileNotFoundException()
        {
            // Arrange
            var fileReader = CreateFileReader();

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() =>
                fileReader.OpenReadAsync(@"C:\missing.json", CancellationToken.None));
        }

        private static FileOpener CreateFileReader()
        {
            var fileSystem = new MockFileSystem();

            return CreateFileReader(fileSystem);
        }

        private static FileOpener CreateFileReader(MockFileSystem fileSystem)
        {
            return new FileOpener(fileSystem);
        }
    }
}
