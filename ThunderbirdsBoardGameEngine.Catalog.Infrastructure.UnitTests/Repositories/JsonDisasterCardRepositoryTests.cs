using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Repositories
{
    public class JsonDisasterCardRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_WhenValidData_ReturnsDisasterCardsAsync()
        {
            // Arrange
            var disasterCards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Test Disaster 1").WithLocation(BoardLocation.NorthAtlantic).WithRescueType(RescueType.Land).WithDifficulty(5).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Test Disaster 2").WithLocation(BoardLocation.SouthPacific).WithRescueType(RescueType.Space).WithDifficulty(7).Build(),
                new DisasterCardBuilder().WithId(3).WithName("Test Disaster 3").WithLocation(BoardLocation.Africa).WithRescueType(RescueType.Air).WithDifficulty(9).Build()
            };

            var jsonText = JsonSerializer.Serialize(disasterCards, JsonDefaults.DisasterCards);

            var repository = CreateRepository(jsonText);

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoData_ReturnsEmptyListAsync()
        {
            // Arrange
            var repository = CreateRepository("[]");

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenNullData_ReturnsEmptyListAsync()
        {
            // Arrange
            var repository = CreateRepository("null");

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyString_ThrowsJsonException()
        {
            // Arrange
            var repository = CreateRepository(string.Empty);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenInvalidJson_ThrowsJsonException()
        {
            // Arrange
            var invalidJson = "{ invalid json }"; // Malformed JSON

            var repository = CreateRepository(invalidJson);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenWhitespaceOnly_ThrowsJsonException()
        {
            // Arrange
            var repo = CreateRepository("   \r\n\t  ");

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repo.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenBonusTypeMissing_ThrowsJsonException()
        {
            // Arrange
            var card = new DisasterCardBuilder().WithId(1).Build();

            var json = JsonSerializer.Serialize(new[] { card }, JsonDefaults.DisasterCards);

            var missingType = json.Replace("\"type\":", "\"typ\":", StringComparison.Ordinal); // valid JSON, wrong key

            var repo = CreateRepository(missingType);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repo.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenInvalidDisasterCards_ThrowsDisasterCardValidationException()
        {
            // Arrange
            var disasterCards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithNullBonusConditions().WithNullRewards().Build(),
                new DisasterCardBuilder().WithId(2).WithName(string.Empty).WithLocation(BoardLocation.SouthPacific).WithRescueType(RescueType.Space).WithDifficulty(7).Build(), // Invalid: Name is empty
                new DisasterCardBuilder().WithId(3).WithName("Another Valid Disaster").WithLocation(BoardLocation.Africa).WithRescueType(RescueType.Air).WithDifficulty(-9).Build()
            };

            var jsonText = JsonSerializer.Serialize(disasterCards, JsonDefaults.DisasterCards);

            var repository = CreateRepository(jsonText);

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenCanceled_ThrowsOperationCanceledException()
        {
            // Arrange
            var path = "/cards.json";

            var options = Options.Create(new DisasterCardJsonOptions 
            { 
                FilePath = path
            });

            var files = new FakeFileReader().AddCanceled(path);

            var repository = new JsonDisasterCardRepository(
                options,
                files);

            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => repository.GetAllAsync(new CancellationToken(canceled: true)));
        }

        private static JsonDisasterCardRepository CreateRepository(string jsonText)
        {
            var options = Options.Create(new DisasterCardJsonOptions
            {
                FilePath = "/cards.json"
            });

            var reader = new FakeFileReader().Add("/cards.json", jsonText);

            return new JsonDisasterCardRepository(options, reader);
        }
    }
}
