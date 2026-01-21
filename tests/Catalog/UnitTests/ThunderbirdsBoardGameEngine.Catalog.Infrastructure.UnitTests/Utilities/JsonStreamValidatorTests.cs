using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Utilities
{
    public class JsonStreamValidatorTests
    {
        private const string TestPath = "test.json";

        [Fact]
        public async Task ValidateStreamAsync_WhenValidContent_ReturnsStream()
        {
            // Arrange
            var content = Encoding.UTF8.GetBytes("{\"key\":23}");

            // Act & Assert
            await AssertValidStream(content);
        }

        [Fact]
        public async Task ValidateStreamAsync_WhenValidJsonArrayContent_ReturnsStream()
        {
            // Arrange
            var validator = CreateValidator();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("[{\"key\":23}, {\"key\":42}]"));

            // Act
            using var validatedStream = await validator.ValidateStreamAsync(stream, TestPath, CancellationToken.None);

            // Assert
            Assert.NotNull(validatedStream);
            Assert.True(validatedStream.CanRead);
            Assert.True(validatedStream.CanSeek);
            Assert.True(validatedStream.Length > 0);

            using var document = await JsonDocument.ParseAsync(validatedStream);
            var elements = document.RootElement.EnumerateArray().ToArray();
            Assert.Equal(2, elements.Length);
            Assert.Equal(23, elements[0].GetProperty("key").GetInt32());
            Assert.Equal(42, elements[1].GetProperty("key").GetInt32());
        }

        [Fact]
        public async Task ValidateStreamAsync_WhenValidContentWithLeadingWhitespace_ReturnsStream()
        {
            // Arrange
            var content = Encoding.UTF8.GetBytes(new string('\n', 8192) + "{\"key\":23}");

            // Act & Assert
            await AssertValidStream(content);
        }

        [Theory]
        [ClassData(typeof(BomOnlyData))]
        public async Task ValidateStreamAsync_WhenValidContentWithLeadingBom_ReturnsStream(string bomOnlyData)
        {
            // Arrange
            var content = Encoding.UTF8.GetBytes(bomOnlyData + "{\"key\":23}");

            // Act & Assert
            await AssertValidStream(content);
        }

        [Fact]
        public async Task ValidateStreamAsync_WhenNullStream_ThrowsInvalidDataExceptionAsync()
        {
            // Arrange
            var validator = CreateValidator();

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => validator.ValidateStreamAsync(null, TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task ValidateStreamAsync_WhenEmptyStream_ThrowsInvalidDataExceptionAsync()
        {
            // Arrange
            var validator = CreateValidator();

            using var emptyStream = new MemoryStream();

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => validator.ValidateStreamAsync(emptyStream, TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Theory]
        [ClassData(typeof(JsonWhiteSpaceData))]
        public async Task ValidateStreamAsync_WhenWhitespaceOnlyStream_ThrowsInvalidDataExceptionAsync(string jsonContent)
        {
            // Arrange
            var validator = CreateValidator();

            using var whitespaceStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonContent));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => validator.ValidateStreamAsync(whitespaceStream, TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Theory]
        [ClassData(typeof(BomOnlyData))]
        public async Task ValidateStreamAsync_WhenWhiteSpaceOnlyWithLeadingBomCharacters_ThrowsInvalidDataException(string bomData)
        {
            // Arrange
            var validator = CreateValidator();

            using var whitespaceStream = new MemoryStream(Encoding.UTF8.GetBytes(bomData + new string('\n', 8192)));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => validator.ValidateStreamAsync(whitespaceStream, TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task ValidateStreamAsync_WhenUnreadableStream_ThrowsInvalidDataExceptionAsync()
        {
            // Arrange
            var validator = CreateValidator();

            using var unreadableStream = new UnreadableStream();

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => validator.ValidateStreamAsync(unreadableStream, TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task ValidateStreamAsync_WhenCanceled_ThrowsOperationCanceledExceptionAsync()
        {
            // Arrange
            var validator = CreateValidator();

            using var stream = new MemoryStream(Encoding.UTF8.GetBytes("{\"key\":23}"));

            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => validator.ValidateStreamAsync(stream, TestPath, cts.Token));
        }

        private static JsonStreamValidator CreateValidator()
        {
            return new JsonStreamValidator();
        }

        private async Task AssertValidStream(byte[] content)
        {
            var validator = CreateValidator();

            using var stream = new MemoryStream(content);

            using var validatedStream = await validator.ValidateStreamAsync(stream, TestPath, CancellationToken.None);

            Assert.NotNull(validatedStream);
            Assert.True(validatedStream.CanRead);
            Assert.True(validatedStream.CanSeek);
            Assert.True(validatedStream.Length > 0);

            using var document = await JsonDocument.ParseAsync(validatedStream);
            Assert.Equal(23, document.RootElement.GetProperty("key").GetInt32());
        }

        /// <summary>
        /// A fake of a stream that does not support reading, writing, or seeking operations.
        /// </summary>
        /// <remarks>All read, write, seek, and flush operations on this stream throw a
        /// NotSupportedException. This class can be used in scenarios where a non-readable, non-writable, and
        /// non-seekable stream is required, such as for testing or as a placeholder.</remarks>
        private sealed class UnreadableStream : Stream
        {
            public override bool CanRead => false;

            public override bool CanSeek => false;

            public override bool CanWrite => false;

            public override long Length => throw new NotSupportedException();

            public override long Position
            {
                get => throw new NotSupportedException();
                set => throw new NotSupportedException();
            }

            public override void Flush()
            {
                throw new NotSupportedException();
            }

            public override int Read(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }

            public override long Seek(long offset, SeekOrigin origin)
            {
                throw new NotSupportedException();
            }

            public override void SetLength(long value)
            {
                throw new NotSupportedException();
            }

            public override void Write(byte[] buffer, int offset, int count)
            {
                throw new NotSupportedException();
            }
        }
    }
}
