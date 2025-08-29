using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.IntegrationTests.Repositories
{
    public class JsonDisasterCardRepositoryTests
    {
        [Fact]
        public async Task GetAllAsync_WhenValidFile_ShouldReturnAllDisasterCardsAsync()
        {
            // Arrange
            var filepath = TestDataPathHelper.GetPath("disaster-cards-test.json");

            var repository = CreateRepository(filepath);

            // Act
            var disasterCards = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(disasterCards);
            Assert.NotEmpty(disasterCards);
            Assert.Equal(2, disasterCards.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoFileExists_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var filepath = "TestData/nonexistent-disaster-cards.json";

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => repository.GetAllAsync());
        }

        [Fact]
        public async Task GetAllAsync_WhenFileIsEmpty_ShouldReturnEmptyListAsync()
        {
            // Arrange
            var filepath = TestDataPathHelper.GetPath("invalid-json.json");

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync());
        }

        [Fact]
        public async Task GetAllAsync_WhenDisasterCardsInvalid_ShouldThrowDisasterCardValidationException()
        {
            // Arrange
            var filepath = TestDataPathHelper.GetPath("invalid-disaster-cards.json");

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(() => repository.GetAllAsync());
        }

        [Fact]
        public async Task GetByIdAsync_WhenDisasterCardExists_ShouldReturnDisasterCardWithExpectedPropertiesAsync()
        {
            // Arrange
            var filepath = TestDataPathHelper.GetPath("disaster-cards-test.json");

            var repository = CreateRepository(filepath);

            // Act
            var disasterCard = await repository.GetByIdAsync(2);

            // Assert
            Assert.NotNull(disasterCard);
            Assert.Equal(2, disasterCard.Id);
            Assert.Equal("Earthquake", disasterCard.Name);
            Assert.Equal(9, disasterCard.DifficultyNumber);
            Assert.Equal(BoardLocation.Asia, disasterCard.Location);
            Assert.Equal(RescueType.Land, disasterCard.RescueType);
            Assert.Equal(2, disasterCard.BonusConditions.Count);
            Assert.Equal(2, disasterCard.RewardOptions.Count);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDisasterCardDoesNotExist_ShouldReturnNullAsync()
        {
            // Arrange
            var filepath = TestDataPathHelper.GetPath("disaster-cards-test.json");

            var repository = CreateRepository(filepath);

            // Act
            var disasterCard = await repository.GetByIdAsync(999);

            // Assert
            Assert.Null(disasterCard);
        }

        private static JsonDisasterCardRepository CreateRepository(string filepath)
        {
            var options = Options.Create(new CardDataOptions
            {
                DisasterCardsFilePath = filepath
            });

            return new JsonDisasterCardRepository(options);
        }
    }
}
