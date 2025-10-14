using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.Services;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Services
{
    public class DisasterCardServiceTests
    {
        [Fact]
        public async Task GetDisasterCards_WhenDisasterCardsExist_ReturnsDisasterCardAsync()
        {
            // Arrange
            var disasterCards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithDifficulty(7).WithSpecifiedReward(BonusToken.Intelligence).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithDifficulty(8).WithLocation(BoardLocation.Asia).WithUserChoiceRewardOption().Build(),
            };

            var repository = Substitute.For<IDisasterCardRepository>();
            repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(disasterCards);

            var service = CreateDisasterCardService(repository);

            // Act
            var result = await service.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(disasterCards.Count, result.Count);
            Assert.All(result, disasterCard => Assert.IsType<DisasterCard>(disasterCard));
            Assert.Same(disasterCards, result);

            await repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]

        public async Task GetAllAsync_WithCancellationToken_Forwarded()
        {
            // Arrange
            var repository = Substitute.For<IDisasterCardRepository>();
            repository.GetAllAsync(Arg.Any<CancellationToken>())
                     .Returns(new List<DisasterCard> { new DisasterCard { Id = 1, Name = "Sample Disaster" } });

            var service = CreateDisasterCardService(repository);

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            var _ = await service.GetAllAsync(cancellationToken);

            // Assert
            await repository.Received(1).GetAllAsync(Arg.Is<CancellationToken>(t => t == cancellationToken));
        }

        [Fact]
        public async Task GetAllAsync_WhenRepositoryThrowsException_Bubbles()
        {
            // Arrange
            var repository = Substitute.For<IDisasterCardRepository>();
            repository.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(Task.FromException<IReadOnlyList<DisasterCard>>(CatalogDataAccessException.DataMissing("some path")));

            var service = CreateDisasterCardService(repository);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => service.GetAllAsync(CancellationToken.None));
            Assert.Equal("some path", exception.Path);

            await repository.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        private static DisasterCardService CreateDisasterCardService(IDisasterCardRepository repository)
        {
            return new DisasterCardService(repository);
        }
    }
}
