using AutoFixture;
using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using System.Threading.Tasks;
using ThunderbirdsBoardGameEngine.GameData.Api.Controllers;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.UnitTests.Controllers
{
    public class DisasterCardControllerTests
    {
        [Fact]
        public async Task Get_WhenDisasterCardsExist_ReturnsOk()
        {
            // Arrange
            var fixture = new Fixture();
            var disasterCardsDtos = fixture.CreateMany<DisasterCardDto>(5).ToList();

            var service = Substitute.For<IDisasterCardService>();
            service.GetAllAsync().Returns(disasterCardsDtos);

            var controller = new DisasterCardController(service);

            // Act
            var result = await controller.Get();

            // Assert
            var okResult = Assert.IsType<OkObjectResult>(result);
            var returnedCards = Assert.IsType<IReadOnlyList<DisasterCardDto>>(okResult.Value, exactMatch: false);
            Assert.NotEmpty(returnedCards);

            await service.Received(1).GetAllAsync();
        }
    }
}
