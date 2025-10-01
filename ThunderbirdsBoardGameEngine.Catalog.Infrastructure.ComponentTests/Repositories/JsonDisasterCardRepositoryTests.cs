using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests.Repositories
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
            var disasterCards = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(disasterCards);
            Assert.NotEmpty(disasterCards);
            Assert.Equal(2, disasterCards.Count);
        }

        [Fact]
        public async Task GetAllAsync_WhenNoFileExists_ShouldThrowFileNotFoundException()
        {
            // Arrange
            var filepath = "nonexistent-disaster-cards.json";

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<FileNotFoundException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenJsonIsInvalid_ShouldThrowJsonException()
        {
            // Arrange
            var filepath = TestDataPathHelper.GetPath("invalid-json.json");

            var repository = CreateRepository(filepath);

            // Act & Assert
            await Assert.ThrowsAsync<JsonException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        private static JsonDisasterCardRepository CreateRepository(string filepath)
        {
            var options = Options.Create(new CardDataOptions
            {
                DisasterCardsFilePath = filepath
            });

            return new JsonDisasterCardRepository(options, new FileReader());
        }
    }
}
