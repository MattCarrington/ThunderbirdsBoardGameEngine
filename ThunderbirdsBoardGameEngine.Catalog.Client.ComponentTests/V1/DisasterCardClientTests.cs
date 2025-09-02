using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.Catalog.WireMock;
using ThunderbirdsBoardGameEngine.TestUtils;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.Fixtures;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.ComponentTests.V1
{
    [Collection("WireMock")]
    public class DisasterCardClientTests
    {
        private readonly IReadOnlyList<DisasterCardDto> _cards = 
            TestDataLoader.LoadJsonFromFile<IReadOnlyList<DisasterCardDto>>("disaster-card-dto-data.json", TestDataConstants.V1InputFolder);

        private readonly WireMockHost _host;
        private readonly IDisasterCardsClient _client;

        public DisasterCardClientTests(WireMockFixture fixture)
        {
            _host = fixture.Host;
            
            var sp = CatalogClientProviderFactory.Build(_host.Url!);
            _client = sp.GetRequiredService<IDisasterCardsClient>();
        }

        [Fact]
        public async Task GetAllAsync_WhenCalled_CallsCorrectRouteOnce()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);

            // Act
            _ = await _client.GetAllAsync();

            // Assert
            var hits = _host.DisasterCardStub.GetAllRequestPaths();

            Assert.Single(hits);
        }

        [Fact]
        public async Task GetAllAsync_WhenValidJson_ReturnsSuccessApiResult()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllSuccess(_cards);
            
            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Equal(_cards.Count, response.Data.Count);
            Assert.Null(response.ErrorMessage);
            DisasterCardDtoAssertions.AssertOrderSensitive(_cards.ToList(), response.Data.ToList());
        }

        [Fact]
        public async Task GetAllAsync_WhenEmptyList_ReturnsSuccessApiResult()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetAllSuccess(new List<DisasterCardDto>());

            // Act
            var response = await _client.GetAllAsync();

            // Assert           
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Empty(response.Data);
            Assert.Null(response.ErrorMessage);
        }

        [Fact]
        public async Task GetByIdAsync_WhenCalled_CallsCorrectRouteOnce()
        {
            // Arrange
            var card = _cards[7];

            _host.DisasterCardStub.RegisterGetByIdSuccess(card);

            // Act
            _ = await _client.GetByIdAsync(card.Id);

            // Assert
            var hits = _host.DisasterCardStub.GetAllRequestPaths();

            Assert.Single(hits);
        }

        [Fact]
        public async Task GetByIdAsync_WhenValidJson_ReturnsSuccessApiResult()
        {
            // Arrange
            var card = _cards[0];

            _host.DisasterCardStub.RegisterGetByIdSuccess(card);

            // Act
            var response = await _client.GetByIdAsync(card.Id);

            // Assert           
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(response.Data);
            Assert.Null(response.ErrorMessage);
            DisasterCardDtoAssertions.AssertEqual(card, response.Data);
        }

        [Fact]
        public async Task GetByIdAsync_WhenRegisteredNotFound_ReturnsFailureApiResult()
        {
            // Arrange
            _host.DisasterCardStub.RegisterGetByIdNotFound();

            // Act
            var response = await _client.GetByIdAsync(9999);

            // Assert
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.Null(response.Data);
            Assert.NotNull(response.ErrorMessage);
            Assert.NotEmpty(response.ErrorMessage);
            Assert.Contains("Disaster card not found.", response.ErrorMessage);
        }
    }
}
