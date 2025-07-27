using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.GameData.Api.Repositories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.DataAccessTests.Repositories
{
    public class DisasterCardRepositoryTests
    {
        [Fact]
        public async Task GetAllDisasterCards_WhenValidFile_ShouldReturnAllDisasterCardsAsync()
        {
            // Arrange
            var filepath = "TestData/disasterCards-test.json";

            var repository = CreateRepository(filepath);

            // Act
            var disasterCards = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(disasterCards);
            Assert.NotEmpty(disasterCards);
            Assert.Equal(2, disasterCards.Count);
        }

        [Fact]
        public async Task GetAllDisasterCards_WhenNoFileExists_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var filepath = "TestData/nonexistent-disasterCards.json";

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => repository.GetAllAsync());
        }

        [Fact]
        public async Task GetAllDisasterCards_WhenFileIsEmpty_ShouldReturnEmptyListAsync()
        {
            // Arrange
            var filepath = "TestData/invalid-json.json";

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync());
        }

        [Fact]
        public async Task GetAllDisasterCards_WhenDisasterCardsInvalid_ShouldThrowDisasterCardValidationException()
        {
            // Arrange
            var filepath = "TestData/invalid-disasterCards.json";

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(() => repository.GetAllAsync());
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
