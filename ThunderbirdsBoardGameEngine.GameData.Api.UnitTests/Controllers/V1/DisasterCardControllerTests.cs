using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ThunderbirdsBoardGameEngine.GameData.Api.Controllers.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Controllers.V1
{
    public class DisasterCardControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly IDisasterCardService _service = Substitute.For<IDisasterCardService>();
        private readonly DisasterCardController _controller;

        public DisasterCardControllerTests()
        {
            _controller = new DisasterCardController(_service);
            _fixture.Customizations.Add(new TypeRelay(typeof(BonusCondition), typeof(CharacterBonusCondition)));
        }

        [Fact]
        public async Task Get_WhenDisasterCardsExist_ReturnsOk()
        {
            // Arrange
            var disasterCards = _fixture.CreateMany<DisasterCard>(5).ToList();

            _service.GetAllAsync().Returns(disasterCards);

            // Act
            var result = await _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCards = Assert.IsType<IReadOnlyList<DisasterCardDto>>(okResult.Value, exactMatch: false);
            Assert.NotEmpty(returnedCards);

            await _service.Received(1).GetAllAsync();
        }

        [Fact]
        public async Task GetById_WhenCardExists_ReturnsOk()
        {
            // Arrange
            var disasterCard = _fixture.Create<DisasterCard>();

            _service.GetByIdAsync(disasterCard.Id).Returns(disasterCard);

            // Act
            var result = await _controller.GetById(disasterCard.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCard = Assert.IsType<DisasterCardDto>(okResult.Value);
            Assert.Equal(disasterCard.Id, returnedCard.Id);
            await _service.Received(1).GetByIdAsync(disasterCard.Id);
        }

        [Fact]
        public async Task GetById_WhenCardDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            _service.GetByIdAsync(nonExistentId).Returns((DisasterCard)null);

            // Act
            var result = await _controller.GetById(nonExistentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains($"Disaster card with ID {nonExistentId} not found.", notFoundResult.Value.ToString());
        }
    }
}