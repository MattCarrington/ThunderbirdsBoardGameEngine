using AutoFixture;
using AutoFixture.Kernel;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Application.Services;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Services
{
    public class DisasterCardServiceTests
    {
        private readonly Fixture _fixture = new();
        private readonly IDisasterCardRepository _repository = Substitute.For<IDisasterCardRepository>();
        private readonly DisasterCardService _service;
        private readonly CancellationToken cancellationToken = CancellationToken.None;

        public DisasterCardServiceTests()
        {
            _service = new DisasterCardService(_repository);
            _fixture.Customizations.Add(new TypeRelay(typeof(BonusCondition), typeof(CharacterBonusCondition)));
        }

        [Fact]
        public async Task GetDisasterCards_WhenDisasterCardsExist_ReturnsDisasterCardAsync()
        {
            // Arrange
            var disasterCards = _fixture.CreateMany<DisasterCard>(5).ToList();
            
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(disasterCards);

            // Act
            var result = await _service.GetAllAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(disasterCards.Count, result.Count);
            Assert.All(result, disasterCard => Assert.IsType<DisasterCard>(disasterCard));

            await _repository.Received(1).GetAllAsync(Arg.Is<CancellationToken>(t => t == cancellationToken));            
        }

        [Fact]
        public async Task GetAllAsync_WhenNoDisasterCardsExist_ReturnsEmptyList()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(Array.Empty<DisasterCard>());

            // Act
            var result = await _service.GetAllAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _repository.Received(1).GetAllAsync(Arg.Is<CancellationToken>(t => t == cancellationToken));
        }

        [Fact]
        public async Task GetAllAsync_WhenDisasterCardsNull_ReturnsEmptyList()
        {
            // Arrange
            _repository.GetAllAsync(Arg.Any<CancellationToken>()).Returns(null as IReadOnlyList<DisasterCard>);

            // Act
            var result = await _service.GetAllAsync(cancellationToken);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _repository.Received(1).GetAllAsync(Arg.Is<CancellationToken>(t => t == cancellationToken));
        }
    }
}
