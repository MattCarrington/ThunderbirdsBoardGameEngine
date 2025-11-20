using AutoFixture;
using AutoFixture.Kernel;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Collections.Immutable;
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
        public void Get_WhenDisasterCardsExist_ReturnsOk()
        {
            // Arrange
            var disasterCards = _fixture.CreateMany<DisasterCard>(5).ToImmutableArray();

            _service.GetAll().Returns(disasterCards);

            // Act
            var result = _controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCards = Assert.IsType<IReadOnlyList<DisasterCardDto>>(okResult.Value, exactMatch: false);
            Assert.NotEmpty(returnedCards);

            _service.Received(1).GetAll();
        }
    }
}