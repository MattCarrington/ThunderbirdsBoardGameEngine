using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Security;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.PayloadReaders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.PayloadReaders
{
    public class ExceptionMappingCatalogPayloadReaderTests
    {
        private const string TestPath = "testpath.json";

        [Fact]
        public async Task ReadAsync_WhenValidPayload_ReturnsPayload()
        {
            // Arrange
            var payload = new GeneratedManifestPayloadBuilder().Build();

            var inner = Substitute.For<ICatalogPayloadReader<GeneratedCatalogManifest>>();
            inner.ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Returns(payload);

            var reader = CreateReader(inner);

            // Act
            var result = await reader.ReadAsync(TestPath, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(payload.RawData, result.RawData);

            await inner.Received(1).ReadAsync(Arg.Is<string>(p => p == TestPath), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsFileNotFoundException_WrapsSourceNotFoundException()
        {
            // Arrange
            var reader = CreateReader(new FileNotFoundException("File not found"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<FileNotFoundException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsDirectoryNotFoundException_WrapsSourceNotFoundException()
        {
            // Arrange
            var reader = CreateReader(new DirectoryNotFoundException("Directory not found"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<DirectoryNotFoundException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsDriveNotFoundException_WrapsSourceNotFoundException()
        {
            // Arrange
            var reader = CreateReader(new DriveNotFoundException("Drive not found"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<DriveNotFoundException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsUnauthorizedAccessException_WrapsAccessDeniedException()
        {
            // Arrange
            var reader = CreateReader(new UnauthorizedAccessException("Access denied"));
            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<UnauthorizedAccessException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsAuthenticationException_WrapsAccessDeniedException()
        {
            // Arrange
            var reader = CreateReader(new AuthenticationException("Authentication failed"));
            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<AuthenticationException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsEndOfStreamException_WrapsSourceUnreadableException()
        {
            // Arrange
            var reader = CreateReader(new EndOfStreamException("End of stream"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<EndOfStreamException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsObjectDisposedException_WrapsSourceUnreadableException()
        {
            // Arrange
            var reader = CreateReader(new ObjectDisposedException("Object disposed"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<ObjectDisposedException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsJsonException_WrapsBadJsonException()
        {
            // Arrange
            var reader = CreateReader(new JsonException("Bad JSON"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<JsonException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_WhenInnerThrowsInvalidDataExceptions_WrapsBadJsonException()
        {
            // Arrange
            var reader = CreateReader(new InvalidDataException("Invalid data"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsInvalidOperationException_WrapsBadJsonException()
        {
            // Arrange
            var reader = CreateReader(new InvalidOperationException("Invalid operation"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidOperationException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsNotSupportedException_WrapsBadJsonException()
        {
            // Arrange
            var reader = CreateReader(new NotSupportedException("Not supported"));
            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<NotSupportedException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsOperationCancelledException_Rethrows()
        {
            // Arrange
            var reader = CreateReader(new OperationCanceledException("Operation cancelled"));

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None));
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsCatalogDataException_Rethrows()
        {
            // Arrange
            var reader = CreateReader(CatalogDataAccessException.SourceNotFound(TestPath, new FileNotFoundException()));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<FileNotFoundException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsDisasterCardValidationException_Rethrows()
        {
            // Arrange
            var reader = CreateReader(DisasterCardValidationException.Unknown());

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None));
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsOutOfMemoryException_Rethrows()
        {
            // Arrange
            var reader = CreateReader(new OutOfMemoryException("Out of memory"));

            // Act & Assert
            await Assert.ThrowsAsync<OutOfMemoryException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None));
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsAccessViolationException_Rethrows()
        {
            // Arrange
            var reader = CreateReader(new AccessViolationException("Access violation"));

            // Act & Assert
            await Assert.ThrowsAsync<AccessViolationException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None));
        }

        [Fact]
        public async Task ReadAsync_InnerThrowsFieldException_WrapsUnknownException()
        {
            // Arrange
            var reader = CreateReader(new FieldAccessException("Generic error"));

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<FieldAccessException>(
                () => reader.ReadAsync(TestPath, CancellationToken.None),
                CatalogDataAccessErrorCode.Unknown,
                TestPath);
        }

        private static ExceptionMappingCatalogPayloadReader<GeneratedCatalogManifest> CreateReader(Exception exception)
        {
            var inner = Substitute.For<ICatalogPayloadReader<GeneratedCatalogManifest>>();
            inner.ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>()).Throws(exception);

            return CreateReader(inner);
        }

        private static ExceptionMappingCatalogPayloadReader<GeneratedCatalogManifest> CreateReader(ICatalogPayloadReader<GeneratedCatalogManifest> inner)
        {
            return new ExceptionMappingCatalogPayloadReader<GeneratedCatalogManifest>(inner);
        }

        private sealed class AuthenticationException(string message) : SecurityException(message)
        {
        }
    }
}
