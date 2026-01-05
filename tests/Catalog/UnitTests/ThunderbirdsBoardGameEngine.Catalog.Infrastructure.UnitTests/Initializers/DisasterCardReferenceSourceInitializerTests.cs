using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initialisers;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Initializers
{
    public class DisasterCardReferenceSourceInitializerTests
    {
        [Fact]
        public async Task BuildAsync_WithValidParameters_ReturnsCatalog()
        {
            // Arrange
            var cards = new List<DisasterCard>
            {
                new DisasterCardBuilder().WithId(1).WithName("Disaster 1").WithCode("disaster-1").WithDifficulty(7).WithSpecifiedReward(BonusToken.Intelligence).Build(),
                new DisasterCardBuilder().WithId(2).WithName("Disaster 2").WithCode("diaster-2").WithDifficulty(8).WithLocation(BoardLocation.Asia).WithUserChoiceRewardOption().Build()
            };

            var reader = Substitute.For<IDisasterCardReader>();
            reader.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(cards));

            var initializer = CreateIntialiser(reader);

            // Act
            var result = await initializer.InitializeAsync(CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("card-2", result.Version);
            Assert.Equal(2, result.Cards.Length);
        }

        [Fact]
        public async Task BuildAsync_WithNoCards_ThrowsInvalidDataException()
        {
            // Arrange
            var reader = Substitute.For<IDisasterCardReader>();
            reader.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(Array.Empty<DisasterCard>()));

            var initializer = CreateIntialiser(reader);

            // Act & Assert
            await Assert.ThrowsAsync<InvalidDataException>(() => initializer.InitializeAsync(CancellationToken.None));
        }

        [Fact]
        public async Task BuildAsync_WithNullCards_ThrowsInvalidDataException()
        {
            // Arrange
            var reader = Substitute.For<IDisasterCardReader>();
            reader.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Task.FromResult<IReadOnlyList<DisasterCard>>(null));

            var initializer = CreateIntialiser(reader);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentNullException>(() => initializer.InitializeAsync(CancellationToken.None));
        }

        private static DisasterCardReferenceSourceInitializer CreateIntialiser(IDisasterCardReader reader)
        {
            return new DisasterCardReferenceSourceInitializer(reader);
        }
    }
}
