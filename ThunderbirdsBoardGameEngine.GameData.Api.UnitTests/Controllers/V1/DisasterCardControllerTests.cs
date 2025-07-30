using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ThunderbirdsBoardGameEngine.GameData.Api.Controllers.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
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
        }

        [Fact]
        public async Task Get_WhenDisasterCardsExist_ReturnsOk()
        {
            // Arrange
            var disasterCardsDtos = _fixture.CreateMany<DisasterCardDto>(5).ToList();

            _service.GetAllAsync().Returns(disasterCardsDtos);

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
            var disasterCardDto = _fixture.Create<DisasterCardDto>();

            _service.GetByIdAsync(disasterCardDto.Id).Returns(disasterCardDto);

            // Act
            var result = await _controller.GetById(disasterCardDto.Id);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCard = Assert.IsType<DisasterCardDto>(okResult.Value);
            Assert.Equal(disasterCardDto.Id, returnedCard.Id);
            await _service.Received(1).GetByIdAsync(disasterCardDto.Id);
        }

        [Fact]
        public async Task GetById_WhenCardDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var nonExistentId = 999;

            _service.GetByIdAsync(nonExistentId).Returns((DisasterCardDto)null);

            // Act
            var result = await _controller.GetById(nonExistentId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Contains($"Disaster card with ID {nonExistentId} not found.", notFoundResult.Value.ToString());
        }
    }
}