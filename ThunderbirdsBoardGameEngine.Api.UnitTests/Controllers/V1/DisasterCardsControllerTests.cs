using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Controllers.V1;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Controllers.V1
{
    public class DisasterCardsControllerTests
    {
        private readonly Fixture _fixture = new();
        private readonly IDisasterCardService _service = Substitute.For<IDisasterCardService>();
        private readonly DisasterCardsController _controller;

        public DisasterCardsControllerTests()
        {
            _controller = new DisasterCardsController(_service);
            _fixture.Customizations.Add(new TypeRelay(typeof(BonusCondition), typeof(CharacterBonusCondition)));
        }

        [Fact]
        public async Task Get_WhenDisasterCardsExist_ReturnsOk()
        {
            // Arrange
            var cancellationToken = CancellationToken.None;

            var disasterCards = _fixture.CreateMany<DisasterCard>(5).ToList();

            _service.GetAllAsync(Arg.Any<CancellationToken>()).Returns(disasterCards);

            // Act
            var result = await _controller.Get(cancellationToken);

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCards = Assert.IsType<IReadOnlyList<DisasterCardDto>>(okResult.Value, exactMatch: false);
            Assert.NotEmpty(returnedCards);

            await _service.Received(1).GetAllAsync(Arg.Is<CancellationToken>(t => t == cancellationToken));
        }
    }
}