using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Decorators;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Decorators
{
    public class ValidatingDisasterCardRepositoryTests
    {

        [Fact]
        public async Task GetAllAsync_WhenValidDisasterCards_ReturnsCards()
        {
            // Arrange
            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithDifficulty(7).WithSpecifiedReward(BonusToken.Intelligence).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithDifficulty(8).WithLocation(BoardLocation.Asia).WithUserChoiceRewardOption().Build()
            };
            
            var inner = Substitute.For<IDisasterCardReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(cards));

            var repository = CreateValidatingDisasterCardRepository(inner);

            // Act
            var result = await repository.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(cards.Count, result.Count);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WithCancellationToken_Forwarded()
        {
            // Arrange
            var inner = Substitute.For<IDisasterCardReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                 .Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(new[] { new DisasterCardBuilder().WithId(1).WithName("OK").Build() }));

            var repository = CreateValidatingDisasterCardRepository(inner);
            using var cancellationToken = new CancellationTokenSource();

            // Act
            var _ = await repository.GetAllAsync(cancellationToken.Token);

            // Assert
            await inner.Received(1).GetAllAsync(Arg.Is(cancellationToken.Token));
        }

        [Fact]
        public async Task GetAllAsync_WhenInvalidDisasterCard_ThrowsDisasterCardValidationException()
        {
            // Arrange
            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithDifficulty(-1).WithSpecifiedReward(BonusToken.Intelligence).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithDifficulty(8).WithLocation(BoardLocation.Asia).WithNullBonusConditions().Build()
            };

            var repository = CreateValidatingDisasterCardRepository(cards);

            // Act & Assert
            await Assert.ThrowsAsync<DisasterCardValidationException>(() => repository.GetAllAsync(CancellationToken.None));
        }

        [Fact]
        public async Task GetAllAsync_WhenInnerThrows_ExceptionBubbles()
        {
            // Arrange
            var repository = CreateValidatingDisasterCardRepository(CatalogDataAccessException.SourceNotFound("/cards.json", new FileNotFoundException()));

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => repository.GetAllAsync(CancellationToken.None));
            Assert.Equal(CatalogDataAccessErrorCode.SourceNotFound, exception.ErrorCode);
        }

        [Fact]
        public async Task GetAllAsync_WhenCancelled_ExceptionBubbles()
        {
            // Arrange
            var repository = CreateValidatingDisasterCardRepository(new OperationCanceledException());

            using var cancellationToken = new CancellationTokenSource();
            cancellationToken.Cancel();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await repository.GetAllAsync(cancellationToken.Token));
        }

        [Fact]
        public async Task GetAllAsync_WhenDataMissing_ExceptionBubbles()
        {
            // Arrange
            var repository = CreateValidatingDisasterCardRepository(CatalogDataAccessException.DataMissing("/cards.json", new InvalidDataException()));

            // Act & Assert
            var ex = await Assert.ThrowsAsync<CatalogDataAccessException>(() => repository.GetAllAsync(CancellationToken.None));
            Assert.Equal(CatalogDataAccessErrorCode.DataMissing, ex.ErrorCode);
        }

        private static ValidatingDisasterCardReader CreateValidatingDisasterCardRepository(IReadOnlyList<DisasterCard> cards)
        {
            var snapshot = cards is DisasterCard[] arr ? arr : cards.ToArray();

            var inner = Substitute.For<IDisasterCardReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(snapshot));

            return CreateValidatingDisasterCardRepository(inner);
        }

        private static ValidatingDisasterCardReader CreateValidatingDisasterCardRepository(Exception ex)
        {
            var inner = Substitute.For<IDisasterCardReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                 .Returns<Task<IReadOnlyList<DisasterCard>>>(_ => throw ex);

            return CreateValidatingDisasterCardRepository(inner);
        }

        private static ValidatingDisasterCardReader CreateValidatingDisasterCardRepository(IDisasterCardReader inner)
        {
            return new ValidatingDisasterCardReader(inner);
        }
    }
}
