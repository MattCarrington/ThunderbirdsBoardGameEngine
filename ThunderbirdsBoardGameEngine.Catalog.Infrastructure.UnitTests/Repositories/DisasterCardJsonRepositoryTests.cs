using Microsoft.Extensions.Options;
using NSubstitute;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Repositories
{
    public class DisasterCardJsonRepositoryTests
    {
        private const string TestPath = "/cards.json";

        [Fact]
        public async Task GetAllAsync_WhenValidData_ReturnsDisasterCards()
        {
            // Arrange
            var disasterCards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Test Disaster 1").WithLocation(BoardLocation.NorthAtlantic).WithRescueType(RescueType.Land).WithDifficulty(5).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Test Disaster 2").WithLocation(BoardLocation.SouthPacific).WithRescueType(RescueType.Space).WithDifficulty(7).Build(),
                new DisasterCardBuilder().WithId(3).WithName("Test Disaster 3").WithLocation(BoardLocation.Africa).WithRescueType(RescueType.Air).WithDifficulty(9).Build()
            };

            var jsonText = SerializeDisasterCardData(disasterCards);

            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(jsonText).Build();

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);

            Assert.Equal("Test Disaster 1", result.FirstOrDefault(x => x.Id == 1).Name);
            Assert.Equal(BoardLocation.SouthPacific, result.FirstOrDefault(x => x.Id == 2).Location);
            Assert.Equal(9, result.FirstOrDefault(x => x.Id == 3).DifficultyNumber);
        }

        [Theory]
        [InlineData("[]")]
        [InlineData("null")]
        public async Task GetAllAsync_WhenNoData_ThowsDataMissingException(string data)
        {
            // Arrange
            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(data).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyString_ThrowsBadJsonException()
        {
            // Arrange
            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(string.Empty).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => repository.GetAllAsync(CancellationToken.None),
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
            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(invalidJson).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenBonusTypeMissing_ThrowsBadJsonException()
        {
            // Arrange
            var card = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).Build(),
                new DisasterCardBuilder().WithId(2).Build()
            };

            var json = SerializeDisasterCardData(card);

            var missingType = json.Replace("\"type\":", "\"typ\":", StringComparison.Ordinal); // valid JSON, wrong key

            var repository = new DisasterCardJsonRepositoryBuilder().WithJson(missingType).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenConverterThrowsNotSupported_ThrowsBadJsonException()
        {
            // Arrange: non-empty JSON so the item converter is invoked
            var json = "[{}]";

            var opts = new JsonSerializerOptions();
            opts.Converters.Add(new ThrowingDisasterCardConverter());

            var jsonOptions = Substitute.For<IOptionsSnapshot<JsonSerializerOptions>>();
            jsonOptions.Get(CatalogJson.Name).Returns(opts);

            var repo = new DisasterCardJsonRepositoryBuilder()
                .WithJson(json)
                .WithFilePath(TestPath)
                .WithJsonOptions(jsonOptions)   // add this to your builder if missing
                .Build();

            // Act & Assert
            await AssertCatalogDataAccessException<NotSupportedException>(
                () => repo.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenFileMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new FileNotFoundException("File not found", TestPath));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<FileNotFoundException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDirectoryMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new DirectoryNotFoundException("Directory not found"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<DirectoryNotFoundException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDriveMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new DriveNotFoundException("Drive not found"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<DriveNotFoundException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceNotFound,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenAccessDenied_WrapsAccessDenied()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new UnauthorizedAccessException("Access denied"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<UnauthorizedAccessException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenSecurityError_WrapsAccessDenied()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new SecurityException("A security issue has occurred"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<SecurityException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenAuthenticationException_WrapsAccessDenied()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new AuthenticationException("Authentication failed"));
            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();
            // Act & Assert
            await AssertCatalogDataAccessException<AuthenticationException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.AccessDenied,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenIOError_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new IOException("IO Error"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<IOException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenEndOfStreamException_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new EndOfStreamException("End of stream"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<EndOfStreamException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenPathTooLong_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new PathTooLongException("Path too long"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<PathTooLongException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenSourceDisposed_WrapsSourceUnreadable()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new ObjectDisposedException("Object has been disposed"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<ObjectDisposedException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.SourceUnreadable,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenGenericError_WrapsUnknown()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new Exception("Generic error"));

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<Exception>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.Unknown,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenException_WrapsUnknown()
        {
            // Arrange
            var fileReader = new FakeFileReader().AddException(TestPath, new FieldAccessException("Generic error"));    //  Random exception type to assert caught by generic handler

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<FieldAccessException>(
                () => repository.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.Unknown,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var files = new FakeFileReader().AddCanceled(TestPath);

            var repository = new DisasterCardJsonRepositoryBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => repository.GetAllAsync(new CancellationToken(canceled: true)));
        }

        [Fact]
        public async Task GetAllAsync_WhenOutOfMemory_Bubbles()
        {
            var files = new FakeFileReader().AddException(TestPath, new OutOfMemoryException("oom"));
            var repo = new DisasterCardJsonRepositoryBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            await Assert.ThrowsAsync<OutOfMemoryException>(() => repo.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenAccessViolation_Bubbles()
        {
            var files = new FakeFileReader().AddException(TestPath, new AccessViolationException("bad"));
            var repo = new DisasterCardJsonRepositoryBuilder().WithFileReader(files).WithFilePath(TestPath).Build();

            await Assert.ThrowsAsync<AccessViolationException>(() => repo.GetAllAsync(CancellationToken.None));
        }

        private static string SerializeDisasterCardData(IList<DisasterCard> disasterCards)
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

        private class AuthenticationException : SecurityException
        {
            public AuthenticationException(string message) : base(message) { }
        }

        private sealed class ThrowingDisasterCardConverter : JsonConverter<DisasterCard>
        {
            public override DisasterCard? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                throw new NotSupportedException("Unsupported type configuration for DisasterCard.");
            }

            public override void Write(Utf8JsonWriter writer, DisasterCard value, JsonSerializerOptions options)
            {
                throw new NotSupportedException("Write not supported.");
            }
        }

    }
}
