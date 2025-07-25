using AutoFixture;
using AutoFixture.Kernel;
using NSubstitute;
using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Services
{
    public class DisasterCardServiceTests
    {
        [Fact]
        public async Task GetDisasterCards_WhenDisasterCardsExist_ReturnsDisasterCardsAsync()
        {
            // Arrange
            var fixture = new Fixture();
            fixture.Customizations.Add(new TypeRelay(typeof(Bonus), typeof(CharacterBonus)));
            var disasterCards = fixture.CreateMany<DisasterCard>(10).ToList();

            var disasterCardRepositorySubstute = Substitute.For<IDisasterCardRepository>();
            disasterCardRepositorySubstute.GetAllAsync().Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(disasterCards));

            var disasterCardService = new DisasterCardService(disasterCardRepositorySubstute);

            // Act
            var result = await disasterCardService.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.NotEmpty(result);
            Assert.Equal(disasterCards.Count, result.Count);
        }
    }
}
