using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Readers
{
    public class DisasterCardJsonReaderTests
    {
        private const string TestPath = "/cards.json";

        [Fact]
        public async Task GetAllAsync_WhenValidData_ReturnsDisasterCards()
        {
            // Arrange
            var disasterCards = new List<DisasterCardCatalogDto>
            {
                new DisasterCardCatalogDtoBuilder().WithId(1).WithName("Test Disaster 1").WithLocation("NorthAtlantic").WithRescueType("Land").WithDifficulty(8).Build(),
                new DisasterCardCatalogDtoBuilder().WithId(2).WithName("Test Disaster 2").WithLocation("SouthPacific").WithRescueType("Space").WithDifficulty(7).Build(),
                new DisasterCardCatalogDtoBuilder().WithId(3).WithName("Test Disaster 3").WithLocation("Africa").WithRescueType("Air").WithDifficulty(9).Build()
            };

            var jsonText = SerializeDisasterCardData(disasterCards);

            var reader = new DisasterCardJsonReaderBuilder().WithJson(jsonText).Build();

            // Act
            var result = await reader.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);

            Assert.Equal("Test Disaster 1", result.FirstOrDefault(x => x.Id == 1).Name);
            Assert.Equal(BoardLocation.SouthPacific, result.FirstOrDefault(x => x.Id == 2).Location);
            Assert.Equal(9, result.FirstOrDefault(x => x.Id == 3).DifficultyNumber);
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceledBeforeCall_ThrowsOperationCanceledException()
        {
            // Arrange
            var reader = new DisasterCardJsonReaderBuilder().WithFilePath(TestPath).Build();

            using var cts = new CancellationTokenSource();
            cts.Cancel();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => reader.GetAllAsync(cts.Token));
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("null")]
        public async Task GetAllAsync_WhenNoData_ThowsDataMissingException(string data)
        {
            // Arrange
            var reader = new DisasterCardJsonReaderBuilder().WithJson(data).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyString_ThrowsBadJsonException()
        {
            // Arrange
            var reader = new DisasterCardJsonReaderBuilder().WithJson(string.Empty).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Theory]
        [InlineData("{ invalid json }")]
        [InlineData("   \r\n\t  ")]
        [InlineData("This is not JSON")]
        [InlineData("")]
        [InlineData("    ")]
        public async Task GetAllAsync_WhenInvalidJson_ThrowsBadJsonException(string invalidJson)
        {
            // Arrange
            var reader = new DisasterCardJsonReaderBuilder().WithJson(invalidJson).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenBonusTypeMissing_ThrowsBadJsonException()
        {
            // Arrange
            var card = new List<DisasterCardCatalogDto>
            {
                new DisasterCardCatalogDtoBuilder().WithId(1).WithLocation("Unknown").Build(),
                new DisasterCardCatalogDtoBuilder().WithId(2).Build()
            };

            var json = SerializeDisasterCardData(card);

            var reader = new DisasterCardJsonReaderBuilder().WithJson(json).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenConverterThrowsNotSupported_ThrowsBadJsonException()
        {
            // Arrange: non-empty JSON so the item converter is invoked
            var json = "[{}]";

            var options = new JsonSerializerOptions();
            options.Converters.Add(new ThrowingDisasterCardConverter());

            var jsonOptions = Substitute.For<IOptionsMonitor<JsonSerializerOptions>>();
            jsonOptions.Get(CatalogJsonDefaults.Name).Returns(options);

            var reader = new DisasterCardJsonReaderBuilder()
                .WithJson(json)
                .WithFilePath(TestPath)
                .WithJsonOptions(jsonOptions)   // add this to your builder if missing
                .Build();

            // Act & Assert
            await AssertCatalogDataAccessException<NotSupportedException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenFileMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new FileNotFoundException("File not found", TestPath));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<FileNotFoundException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDirectoryMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new DirectoryNotFoundException("Directory not found"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<DirectoryNotFoundException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDriveMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new DriveNotFoundException("Drive not found"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<DriveNotFoundException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenAccessDenied_WrapsAccessDenied()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new UnauthorizedAccessException("Access denied"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<UnauthorizedAccessException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenSecurityError_WrapsAccessDenied()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new SecurityException("A security issue has occurred"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<SecurityException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenAuthenticationException_WrapsAccessDenied()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new AuthenticationException("Authentication failed"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<AuthenticationException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenIOError_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new IOException("IO Error"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<IOException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenEndOfStreamException_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new EndOfStreamException("End of stream"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<EndOfStreamException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenPathTooLong_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new PathTooLongException("Path too long"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<PathTooLongException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenSourceDisposed_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new ObjectDisposedException("Object has been disposed"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<ObjectDisposedException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenGenericError_WrapsUnknown()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new Exception("Generic error"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<Exception>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.Unknown,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenException_WrapsUnknown()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new FieldAccessException("Generic error"));    //  Random exception type to assert caught by generic handler

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<FieldAccessException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.Unknown,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var files = new FakeFileOpener().AddCanceled(TestPath);

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => reader.GetAllAsync(new CancellationToken(canceled: true)));
        }

        [Fact]
        public async Task GetAllAsync_WhenOutOfMemory_Bubbles()
        {
            // Arrange
            var files = new FakeFileOpener().AddException(TestPath, new OutOfMemoryException("boom"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAsync<OutOfMemoryException>(() => reader.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenAccessViolation_Bubbles()
        {
            // Arrange
            var files = new FakeFileOpener().AddException(TestPath, new AccessViolationException("bad"));
            var reader = new DisasterCardJsonReaderBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAsync<AccessViolationException>(() => reader.GetAllAsync(CancellationToken.None));
        }

        private static string SerializeDisasterCardData(IList<DisasterCardCatalogDto> disasterCards)
        {
            return JsonSerializer.Serialize(disasterCards, JsonDefaults.DisasterCards);
        }

        private static async Task AssertCatalogDataAccessException<TInner>(Func<Task> action, CatalogDataAccessErrorCode expectedErrorCode, string path) where TInner : Exception
        {
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(action);
            Assert.Equal(expectedErrorCode, exception.ErrorCode);
            Assert.Equal(path, exception.Path);
            Assert.IsType<TInner>(exception.InnerException);
        }

        private class AuthenticationException(string message) : SecurityException(message)
        {
        }

        private sealed class ThrowingDisasterCardConverter : JsonConverter<DisasterCardCatalogDto>
        {
            public override DisasterCardCatalogDto? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotSupportedException("Unsupported type configuration for DisasterCard.");
            }

            public override void Write(Utf8JsonWriter writer, DisasterCardCatalogDto value, JsonSerializerOptions options)
            {
                throw new NotSupportedException("Write not supported.");
            }
        }

    }
}
