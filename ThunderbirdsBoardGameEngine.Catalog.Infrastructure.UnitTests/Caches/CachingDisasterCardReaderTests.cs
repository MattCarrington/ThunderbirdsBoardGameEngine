using NSubstitute;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Caches;
using ThunderbirdsBoardGameEngine.TestUtils.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Caches
{
    public class CachingDisasterCardReaderTests
    {
        private static readonly ImmutableArray<DisasterCard> _cards =
        [
            new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithDifficulty(5).WithSpecifiedReward(BonusToken.Intelligence).Build(),
            new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithDifficulty(8).WithLocation(BoardLocation.Asia).WithUserChoiceRewardOption().Build()
        ];

        [Fact]
        public async Task GetAllAsync_WhenCalledTwice_InvokesInnerReaderOnlyOnce()
        {
            // Arrange
            var inner = CreateInner(_cards);

            var cache = CreateCache(inner);

            // Act
            var first = await cache.GetAllAsync(CancellationToken.None);
            var second = await cache.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.Same(first, second);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WithCancellationToken_StillReturnsCachedResult()
        {
            // Arrange
            var inner = CreateInner(_cards);

            var cache = CreateCache(inner);

            using var cancellationTokenSource = new CancellationTokenSource();
            var cancellationToken = cancellationTokenSource.Token;

            // Act
            var result = await cache.GetAllAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(_cards.ToList().Count, result.Count);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenMultipleCalls_Concurrency()
        {
            // Arrange
            var inner = CreateInner(_cards);

            var cache = CreateCache(inner);

            var tasks = Enumerable.Range(0, 100)
                                  .Select(_ => cache.GetAllAsync(CancellationToken.None))
                                  .ToArray();
            // Act
            var results = await Task.WhenAll(tasks);

            // Assert
            foreach (var result in results)
            {
                Assert.NotNull(result);
                Assert.Equal(_cards.ToList().Count, result.Count);
                
            }

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenInnerThrows_ThrowsException()
        {
            // Arrange
            var inner = Substitute.For<IDisasterCardReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                 .Returns<Task<IReadOnlyList<DisasterCard>>>(_ => throw new InvalidOperationException("Inner reader failure"));

            var cache = CreateCache(inner);

            // Act & Assert
            var first = await Assert.ThrowsAsync<InvalidOperationException>(() => cache.GetAllAsync(CancellationToken.None));
            var second = await Assert.ThrowsAsync<InvalidOperationException>(() => cache.GetAllAsync(CancellationToken.None));

            Assert.Same(first, second);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenNotCalled_NoEagerLoad()
        {
            // Arrange
            var inner = CreateInner(_cards);

            var _ = CreateCache(inner);

            // Act
            // No call to GetAllAsync

            // Assert
            await inner.DidNotReceive().GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenCalledAfterDelay_InvokesInnerOnlyOnce()
        {
            // Arrange
            var inner = CreateInner(_cards, delay: 100);

            var cache = CreateCache(inner);

            // Act
            await Task.Delay(200); // Wait to ensure any lazy loading would have occurred

            var first = await cache.GetAllAsync(CancellationToken.None);
            var second = await cache.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(first);
            Assert.Same(first, second);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenNoData_ReturnsNoData()
        {
            // Arrange
            var inner = CreateInner([]);

            var cache = CreateCache(inner);

            // Act
            var result = await cache.GetAllAsync(CancellationToken.None);
            
            // Assert
            Assert.Empty(result);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WhenInnerReturnsChanges_ReturnsOriginalList()
        {
            // Arrange
            var mutable = new List<DisasterCard>(_cards);

            var inner = CreateInner(mutable);

            var cache = CreateCache(inner);

            var first = await cache.GetAllAsync(CancellationToken.None);
            mutable.Clear();

            // Act
            var second = await cache.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(second);
            Assert.Same(first, second);
            Assert.Equal(_cards.ToList().Count, second.Count);
        }

        private static IDisasterCardReader CreateInner(IReadOnlyList<DisasterCard> cards)
        {
            return CreateInner(cards, 0);
        }

        private static IDisasterCardReader CreateInner(IReadOnlyList<DisasterCard> cards, int delay)
        {
            var inner = Substitute.For<IDisasterCardReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                 .Returns(async _ =>
                 {
                     await Task.Delay(delay);
                     return cards;
                 });

            return inner;
        }

        private static CachingDisasterCardReader CreateCache(IDisasterCardReader inner)
        {
            return new CachingDisasterCardReader(inner);
        }
    }
}