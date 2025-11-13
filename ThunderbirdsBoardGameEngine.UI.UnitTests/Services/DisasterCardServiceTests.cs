using AutoFixture;
using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.UI.Services;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.UnitTests.Services
{
    public class DisasterCardServiceTests
    {
        [Fact]
        public async Task GetAllAsync_WhenResponseIsSuccessful_ReturnsListOfDisasterCards()
        {
            // Arrange
            var fixture = new Fixture();

            var disasterCards = fixture.Create<IReadOnlyList<DisasterCardDto>>();

            var response = ApiResult<IReadOnlyList<DisasterCardDto>>.SuccessResult(disasterCards, HttpStatusCode.OK);

            var service = CreateService(response);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertOrderSensitive(disasterCards.ToList(), result.ToList());
        }

        [Fact]
        public async Task GetAllAsync_WhenResponseIsNotSuccessful_ReturnsEmptyList()
        {
            // Arrange
            var fixture = new Fixture();
            var errorMessage = fixture.Create<string>();

            var response = ApiResult<IReadOnlyList<DisasterCardDto>>.Failure(errorMessage, HttpStatusCode.BadRequest);

            var service = CreateService(response);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        [Fact]
        public async Task GetAllAsync_WhenClientReturnsNullData_ReturnsEmptyList()
        {
            // Arrange
            var response = ApiResult<IReadOnlyList<DisasterCardDto>>.SuccessResult(null, HttpStatusCode.OK);

            var service = CreateService(response);

            // Act
            var result = await service.GetAllAsync();

            // Assert
            Assert.Empty(result);
        }

        private static DisasterCardService CreateService(ApiResult<IReadOnlyList<DisasterCardDto>> apiResult)
        {
            var client = Substitute.For<IDisasterCardsClient>();
            client.GetAllAsync().Returns(Task.FromResult(apiResult));
            
            return new DisasterCardService(client);
        }
    }
}
