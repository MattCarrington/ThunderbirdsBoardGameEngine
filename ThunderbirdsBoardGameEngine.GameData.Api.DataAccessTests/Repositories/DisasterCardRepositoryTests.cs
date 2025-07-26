using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.GameData.Api.Repositories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.DataAccessTests.Repositories
{
    public class DisasterCardRepositoryTests
    {
        [Fact]
        public async Task GetAllDisasterCards_ShouldReturnAllDisasterCardsAsync()
        {
            // Arrange
            var options = Options.Create(new CardDataOptions
            {
                DisasterCardFilePath = "TestData/disasterCards-test.json"
            });

            var repository = new DisasterCardRepository(options);

            // Act
            var disasterCards = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(disasterCards);
            Assert.NotEmpty(disasterCards);
            Assert.Equal(2, disasterCards.Count);
        }
    }
}
