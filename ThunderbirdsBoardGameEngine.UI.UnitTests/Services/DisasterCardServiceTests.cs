using AutoFixture;
using NSubstitute;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
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

        [Fact]
        public async Task GetByIdAsync_WhenResponseIsSuccessful_ReturnsDisasterCard()
        {
            // Arrange
            var fixture = new Fixture();
            var disasterCard = fixture.Create<DisasterCardDto>();

            var response = ApiResult<DisasterCardDto>.SuccessResult(disasterCard, HttpStatusCode.OK);

            var service = CreateService(response);

            // Act
            var result = await service.GetByIdAsync(disasterCard.Id);

            // Assert
            Assert.NotNull(result);
            DisasterCardDtoAssertions.AssertEqual(disasterCard, result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenResponseIsNotSuccessful_ReturnsNull()
        {
            // Arrange
            var response = ApiResult<DisasterCardDto>.Failure("Error", HttpStatusCode.BadRequest);

            var service = CreateService(response);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        [Fact]
        public async Task GetByIdAsync_WhenClientReturnsNullData_ReturnsNull()
        {
            // Arrange
            var response = ApiResult<DisasterCardDto>.SuccessResult(null, HttpStatusCode.OK);

            var service = CreateService(response);

            // Act
            var result = await service.GetByIdAsync(1);

            // Assert
            Assert.Null(result);
        }

        private static DisasterCardService CreateService(ApiResult<DisasterCardDto> apiResult)
        {
            var client = Substitute.For<IDisasterCardsClient>();
            client.GetByIdAsync(Arg.Any<int>()).Returns(Task.FromResult(apiResult));

            return new DisasterCardService(client);
        }

        private static DisasterCardService CreateService(ApiResult<IReadOnlyList<DisasterCardDto>> apiResult)
        {
            var client = Substitute.For<IDisasterCardsClient>();
            client.GetAllAsync().Returns(Task.FromResult(apiResult));
            
            return new DisasterCardService(client);
        }
    }
}
