using ThunderbirdsBoardGameEngine.GameData.Api.Repositories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.DataAccessTests
{
    public class DisasterCardRepositoryTests
    {
        [Fact]
        public async Task GetAllDisasterCards_ShouldReturnAllDisasterCardsAsync()
        {
            // Arrange
            var filepath = Path.Combine("Data", "DisasterCards-test.json");

            var repository = new DisasterCardRepository(filepath);

            // Act
            var disasterCards = await repository.GetAllAsync();

            // Assert
            Assert.NotNull(disasterCards);
            Assert.NotEmpty(disasterCards);
            Assert.Equal(2, disasterCards.Count);
        }
    }
}
