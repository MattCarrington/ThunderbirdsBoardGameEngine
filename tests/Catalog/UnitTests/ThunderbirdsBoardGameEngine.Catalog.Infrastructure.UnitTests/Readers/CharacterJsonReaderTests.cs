using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Readers
{
    public class CharacterJsonReaderTests
    {
        private const string TestPath = "/characters.json";

        [Fact]
        public async Task GetAllAsync_WhenValidData_ReturnsMappedCharacters()
        {
            // Arrange
            var dtos = ValidCharacters();

            var characters = new List<CharacterDefinition>{
                new(Character.Scott, new CharacterRescueBonus(RescueType.Sea, 2)),
                new(Character.LadyPenelope)
            };

            var payload = new Payload<SimpleCatalogManifest>
            {
                Manifest = new SimpleCatalogManifest
                {
                    Catalog = "Characters",
                    SchemaVersion = "1.0",
                    ContentVersion = "1.0.0"
                },
                RawData = JsonDocument.Parse(JsonSerializer.Serialize(dtos)).RootElement.Clone()
            };

            var payloadReader = Substitute.For<ICatalogPayloadReader<SimpleCatalogManifest>>();
            payloadReader.ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(payload);

            var deserializer = Substitute.For<ICharacterDeserializer>();
            deserializer.Deserialize(payload.RawData).Returns(dtos);

            var mapper = Substitute.For<ICharacterMapper>();
            mapper.Map(dtos[0]).Returns(characters[0]);
            mapper.Map(dtos[1]).Returns(characters[1]);

            var reader = CreateReader(payloadReader, deserializer,  mapper);

            // Act
            var result = await reader.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(2, result.Count);
            Assert.Same(characters[0], result[0]);
            Assert.Same(characters[1], result[1]);

            await payloadReader.Received(1).ReadAsync(Arg.Is<string>(f => f == TestPath), Arg.Any<CancellationToken>());
            deserializer.Received(1).Deserialize(payload.RawData);
            mapper.Received(1).Map(dtos[0]);
            mapper.Received(1).Map(dtos[1]);
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceledBeforeCall_ThrowsOperationCanceledException()
        {
            // Arrange
            var reader = CreateReader();

            using var cts = new CancellationTokenSource();
            await cts.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => reader.GetAllAsync(cts.Token));
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerReturnsNoData_ThowsDataMissingException()
        {
            // Arrange
            var deserializer = Substitute.For<ICharacterDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns([]);

            var reader = CreateReader(
                deserializer: deserializer);

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerReturnsNull_ThrowsDataMissingException()
        {
            // Arrange
            var deserializer = Substitute.For<ICharacterDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(null as List<CharacterCatalogDto>);

            var reader = CreateReader(
                deserializer: deserializer);

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerReturnsListContainingNull_ThrowsBadJsonException()
        {
            // Arrange
            var data = new List<CharacterCatalogDto>
            {
                null!
            };

            data.AddRange(ValidCharacters());

            var deserializer = Substitute.For<ICharacterDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(data.ToList());

            var reader = CreateReader(
                deserializer: deserializer);

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.DataMissing,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerThrowsNotSupportedException_WrapsBadJsonException()
        {
            // Arrange
            var deserializer = Substitute.For<ICharacterDeserializer    >();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Throws(new NotSupportedException("Unsupported type configuration for DisasterCard."));

            var reader = CreateReader(
                deserializer: deserializer);

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<NotSupportedException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        [Fact]
        public async Task GetAllAsync_WhenDeserializerThrowsJsonException_WrapsBadJsonException()
        {
            // Arrange
            var deserializer = Substitute.For<ICharacterDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Throws(new JsonException("Malformed JSON data."));

            var reader = CreateReader(
                deserializer: deserializer);

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<JsonException>(
                () => reader.GetAllAsync(CancellationToken.None),
                CatalogDataAccessErrorCode.BadJson,
                TestPath);
        }

        private CharacterJsonReader CreateReader(
            ICatalogPayloadReader<SimpleCatalogManifest>? payloadReader = null,
            ICharacterDeserializer? deserializer = null,
            ICharacterMapper? mapper = null)
        {
            return new CharacterJsonReader(
                Options.Create(new CharacterJsonOptions { FilePath = TestPath }),
                payloadReader ?? CreateDefaultPayloadReader(),
                deserializer ?? Substitute.For<ICharacterDeserializer>(),
                mapper ?? Substitute.For<ICharacterMapper>(),
                NullLogger<CharacterJsonReader>.Instance);
        }

        private static ICatalogPayloadReader<SimpleCatalogManifest> CreateDefaultPayloadReader()
        {
            var payload = new Payload<SimpleCatalogManifest>
            {
                Manifest = new SimpleCatalogManifest
                {
                    Catalog = "DisasterCards",
                    SchemaVersion = "1.0",
                    ContentVersion = "1.0.0"
                },
                RawData = JsonDocument.Parse("[]").RootElement.Clone()
            };

            var reader = Substitute.For<ICatalogPayloadReader<SimpleCatalogManifest>>();

            reader.ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(payload);

            return reader;
        }

        private static List<CharacterCatalogDto> ValidCharacters()
        {
            return new List<CharacterCatalogDto>
            {
                new() {
                    Key = "scott",
                    RescueBonuses = new List<CharacterRescueBonusCatalogDto>
                    {
                        new() {
                            RescueType = "sea",
                            BonusValue = 2
                        }
                    }
                },
                new() {
                    Key = "ladyp",
                    RescueBonuses = []
                }
            };
        }
    }
}
