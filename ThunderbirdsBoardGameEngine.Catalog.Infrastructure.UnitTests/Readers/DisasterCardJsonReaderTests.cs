using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Security;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using ThunderbirdsBoardGameEngine.TestUtils.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Readers
{
    public class DisasterCardJsonReaderTests
    {
        private const string TestPath = "/cards.json";

        [Fact]
        public async Task GetAllAsync_WhenValidData_ReturnsMappedDisasterCards()
        {
            // Arrange
            var disasterCards = ValidCards();

            var stream = new EnvelopeStreamBuilder()
                .WithData(SerializeDisasterCardData(disasterCards))
                .CreateStream();

            var file = Substitute.For<IFileOpener>();
            file.OpenReadAsync(Arg.Is<string>(p => p == TestPath), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Stream>(stream));

            var payload = new Payload
            {
                Manifest = new CatalogManifest
                {
                    Catalog = "DisasterCards",
                    SchemaVersion = "1.0",
                    ContentVersion = "1.0.0",
                    GeneratedAtUtc = DateTime.UtcNow,
                    ItemCount = disasterCards.Count,
                    Checksum = new Checksum
                    {
                        Algorithm = "SHA256",
                        Value = "dummychecksumvalue"
                    }
                },
                RawData = JsonDocument.Parse(SerializeDisasterCardData(disasterCards)).RootElement.Clone()
            };

            var parser = Substitute.For<IEnvelopeParser>();
            parser.ReadEnvelopeAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(payload));

            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(disasterCards);

            var reader = new DisasterCardJsonReaderBuilder()
                .WithFileOpener(file)
                .WithEnvelopeParser(parser)
                .WithDeserializer(deserializer)
                .WithFilePath(TestPath)
                .Build();

            // Act
            var result = await reader.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);
            
            await file.Received(1).OpenReadAsync(Arg.Is<string>(p => p == TestPath), Arg.Any<CancellationToken>());
            await parser.Received(1).ReadEnvelopeAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>());
            deserializer.Received(1).Deserialize(Arg.Any<JsonElement>());
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceledBeforeCall_ThrowsOperationCanceledException()
        {
            // Arrange
            var reader = new DisasterCardJsonReaderBuilder().WithFilePath(TestPath).Build();

            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => reader.GetAllAsync(cts.Token));
        }

        [Fact]
        public async Task GetAllAsync_WhenStreamReturnsEmpty_ThrowsDataMissingException()
        {
            // Arrange
            var opener = Substitute.For<IFileOpener>();
            opener.OpenReadAsync(Arg.Is<string>(p => p == TestPath), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Stream>(new MemoryStream()));

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(opener).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenStreamReturnsNull_ThrowsDataMissingException()
        {
            // Arrange
            var opener = Substitute.For<IFileOpener>();
            opener.OpenReadAsync(Arg.Is<string>(p => p == TestPath), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult<Stream>(null!));

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(opener).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Theory]
        [ClassData(typeof(JsonEmptyStringData))]
        public async Task GetAllAsync_WhenStreamReturnsWhitespace_ThrowsDataMissingException(string jsonContent)
        {
            // Arrange
            var opener = new FakeFileOpener().Add(TestPath, jsonContent);

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(opener).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerReturnsNoData_ThowsDataMissingException()
        {
            // Arrange
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns([]);

            var reader = new DisasterCardJsonReaderBuilder().WithDeserializer(deserializer).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerReturnsNull_ThrowsDataMissingException()
        {
            // Arrange
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(null as List<DisasterCardCatalogDto>);

            var reader = new DisasterCardJsonReaderBuilder().WithDeserializer(deserializer).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerReturnsListContainingNull_ThrowsBadJsonException()
        {
            // Arrange
            var data = new List<DisasterCardCatalogDto>
            {
                null!
            };

            data.AddRange(ValidCards());

            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(data.ToList());

            var reader = new DisasterCardJsonReaderBuilder().WithDeserializer(deserializer).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDtosCountDoesNotMatchManifest_ThrowsBadJson()
        {
            // Arrange
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(ValidCards());

            var reader = new DisasterCardJsonReaderBuilder().WithDeserializer(deserializer).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }
                
        [Fact]
        public async Task GetAllAsync_WhenFileMissing_WrapsSourceNotFound()
        {
            // Arrange
            var fileReader = new FakeFileOpener().AddException(TestPath, new FileNotFoundException("File not found", TestPath));

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

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

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(fileReader).WithFilePath(TestPath).Build();

            // Act & Assert
            await AssertCatalogDataAccessException<FieldAccessException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.Unknown,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenFileReaderCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var files = new FakeFileOpener().AddCanceled(TestPath);

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(files).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => reader.GetAllAsync(new CancellationToken(canceled: true)));
        }

        [Fact]
        public async Task GetAllAsync_WhenOutOfMemory_Bubbles()
        {
            // Arrange
            var files = new FakeFileOpener().AddException(TestPath, new OutOfMemoryException("boom"));

            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(files).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAsync<OutOfMemoryException>(() => reader.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenAccessViolation_Bubbles()
        {
            // Arrange
            var files = new FakeFileOpener().AddException(TestPath, new AccessViolationException("bad"));
            var reader = new DisasterCardJsonReaderBuilder().WithFileOpener(files).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAsync<AccessViolationException>(() => reader.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenParserThrowsInvalidDataException_WrapsBadJsonException()
        {
            // Arrange
            var parser = Substitute.For<IEnvelopeParser>();
            parser.ReadEnvelopeAsync(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
                .Throws(new InvalidDataException("Malformed envelope data."));

            var reader = new DisasterCardJsonReaderBuilder()
                .WithEnvelopeParser(parser)
                .WithFilePath(TestPath)
                .Build();

            // Act & Assert
            await AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerThrowsNotSupportedException_WrapsBadJsonException()
        {
            // Arrange
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Throws(new NotSupportedException("Unsupported type configuration for DisasterCard."));

            var reader = new DisasterCardJsonReaderBuilder()
                .WithDeserializer(deserializer)
                .WithFilePath(TestPath)
                .Build();

            // Act & Assert
            await AssertCatalogDataAccessException<NotSupportedException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerThrowsJsonException_WrapsBadJsonException()
        {
            // Arrange
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Throws(new JsonException("Malformed JSON data."));

            var reader = new DisasterCardJsonReaderBuilder()
                .WithDeserializer(deserializer)
                .WithFilePath(TestPath)
                .Build();

            // Act & Assert
            await AssertCatalogDataAccessException<JsonException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenMapperThrowsDisasterCardValidationException()
        {
            // Arrange
            var mapper = Substitute.For<IDisasterCardMapper>();
            mapper.Map(Arg.Any<DisasterCardCatalogDto>())
                .Throws(DisasterCardValidationException.Unknown());

            var reader = new DisasterCardJsonReaderBuilder().WithMapper(mapper).WithFilePath(TestPath).Build();

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(() => reader.GetAllAsync(CancellationToken.None));
        }

        private static List<DisasterCardCatalogDto> ValidCards()
        {
            return
            [
                new DisasterCardCatalogDtoBuilder().WithId(1).WithName("Test Disaster 1").WithLocation("NorthAtlantic").WithRescueType("Land").WithDifficulty(8).Build(),
                new DisasterCardCatalogDtoBuilder().WithId(2).WithName("Test Disaster 2").WithLocation("SouthPacific").WithRescueType("Space").WithDifficulty(7).Build(),
                new DisasterCardCatalogDtoBuilder().WithId(3).WithName("Test Disaster 3").WithLocation("Africa").WithRescueType("Air").WithDifficulty(9).Build()
            ];
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

        private sealed class AuthenticationException(string message) : SecurityException(message)
        {
        }
    }
}
