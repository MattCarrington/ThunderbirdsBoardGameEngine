using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using NSubstitute.ExceptionExtensions;
using System.Security;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
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

            var payload = new GeneratedManifestPayloadBuilder()
                .WithItemCount(disasterCards.Count)
                .WithDisasterCards(disasterCards)
                .Build();

            var payloadReader = Substitute.For<ICatalogPayloadReader<GeneratedCatalogManifest>>();
            payloadReader.ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(payload));

            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(disasterCards);

            var reader = CreateReader(
                payloadReader: payloadReader,
                deserializer: deserializer);

            // Act
            var result = await reader.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);

            await payloadReader.Received(1).ReadAsync(Arg.Is<string>(p => p == TestPath), Arg.Any<CancellationToken>());
            deserializer.Received(1).Deserialize(Arg.Any<JsonElement>());
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
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
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
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(null as List<DisasterCardCatalogDto>);

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
            var data = new List<DisasterCardCatalogDto>
            {
                null!
            };

            data.AddRange(ValidCards());

            var deserializer = Substitute.For<IDisasterCardDeserializer>();
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
        public async Task GetAllAsync_WhenDtosCountDoesNotMatchManifest_ThrowsBadJson()
        {
            // Arrange
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(ValidCards());

            var reader = CreateReader(
                deserializer: deserializer);

            // Act & Assert
            await CatalogDataAccessAssertions.AssertCatalogDataAccessException<InvalidDataException>(
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
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
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

        private static List<DisasterCardCatalogDto> ValidCards()
        {
            return
            [
                new DisasterCardCatalogDtoBuilder().WithId(1).WithName("Test Disaster 1").WithLocation("NorthAtlantic").WithRescueType("Land").WithDifficulty(8).Build(),
                new DisasterCardCatalogDtoBuilder().WithId(2).WithName("Test Disaster 2").WithLocation("SouthPacific").WithRescueType("Space").WithDifficulty(7).Build(),
                new DisasterCardCatalogDtoBuilder().WithId(3).WithName("Test Disaster 3").WithLocation("Africa").WithRescueType("Air").WithDifficulty(9).Build()
            ];
        }

        private DisasterCardJsonReader CreateReader(
            ICatalogPayloadReader<GeneratedCatalogManifest>? payloadReader = null,
            IDisasterCardDeserializer? deserializer = null,
            IDisasterCardMapper? mapper = null)
        {
            return new DisasterCardJsonReader(
                Options.Create(new DisasterCardJsonOptions { FilePath = TestPath }),
                payloadReader ?? CreateDefaultPayloadReader(),
                deserializer ?? Substitute.For<IDisasterCardDeserializer>(),
                mapper ?? Substitute.For<IDisasterCardMapper>(),
                NullLogger<DisasterCardJsonReader>.Instance);
        }

        private static ICatalogPayloadReader<GeneratedCatalogManifest> CreateDefaultPayloadReader()
        {
            var reader = Substitute.For<ICatalogPayloadReader<GeneratedCatalogManifest>>();

            reader.ReadAsync(Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(new GeneratedManifestPayloadBuilder().Build());

            return reader;
        }
    }
}
