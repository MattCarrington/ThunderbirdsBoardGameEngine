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
            
            _repository.GetAllAsync().Returns(disasterCards);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Equal(disasterCards.Count, result.Count);
            Assert.All(result, disasterCard => Assert.IsType<DisasterCard>(disasterCard));

            await _repository.Received(1).GetAllAsync();            
        }

        [Fact]
        public async Task GetAllAsync_WhenNoDisasterCardsExist_ReturnsEmptyList()
        {
            // Arrange
            _repository.GetAllAsync().Returns(Array.Empty<DisasterCard>());

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _repository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task GetAllAsync_WhenDisasterCardsNull_ReturnsEmptyList()
        {
            // Arrange
            _repository.GetAllAsync().Returns(null as IReadOnlyList<DisasterCard>);

            // Act
            var result = await _service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);

            await _repository.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardExists_ReturnsDisasterCard()
        {
            // Arrange
            var disasterCard = _fixture.Create<DisasterCard>();

            _repository.GetByIdAsync(disasterCard.Id).Returns(disasterCard);

            // Act
            var result = await _service.GetByIdAsync(disasterCard.Id);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<DisasterCard>(result);

            await _repository.Received(1).GetByIdAsync(disasterCard.Id);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCardDoesNotExist_ReturnsNull()
        {
            // Arrange
            int nonExistentId = 999;

            _repository.GetByIdAsync(nonExistentId).Returns((DisasterCard)null);

            // Act
            var result = await _service.GetByIdAsync(nonExistentId);

            // Assert
            Assert.Null(result);
            await _repository.Received(1).GetByIdAsync(nonExistentId);
        }
    }
}
