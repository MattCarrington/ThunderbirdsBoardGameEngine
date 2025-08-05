using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Repositories;
using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;
using ThunderbirdsBoardGameEngine.GameData.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.GameData.Api.DataAccess.IntegrationTests.Repositories
{
    public class DisasterCardRepositoryTests
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

        private static DisasterCardRepository CreateRepository(string filepath)
        {
            var options = Options.Create(new CardDataOptions
            {
                DisasterCardsFilePath = filepath
            });

            return new DisasterCardRepository(options);
        }
    }
}
