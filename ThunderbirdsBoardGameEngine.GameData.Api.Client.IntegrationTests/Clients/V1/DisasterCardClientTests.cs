using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.IntegrationTests.Clients.V1
{
    public class DisasterCardClientTests : IClassFixture<TestServerFixture>
    {
        private readonly IDisasterCardClient _client;
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public DisasterCardClientTests(TestServerFixture testServerFixture)
        {
            _client = testServerFixture.Client;
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllDisasterCards()
        {
            // Arrange

            // Act
            var result = await _client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.IsType<IReadOnlyList<DisasterCardDto>>(result.Data, exactMatch: false);
            Assert.NotNull(result.Data);
            Assert.NotEmpty(result.Data);
            Assert.Null(result.ErrorMessage);

            var expected = TestDataLoader.LoadJsonFromFile<List<DisasterCardDto>>("DisasterCardDtos.json");

            for (int i = 0; i < expected.Count; i++)
            {
                DisasterCardDtoAssertions.AssertDisasterCardDtoEqual(expected[i], result.Data[i]);
            }
        }

        [Fact]
        public async Task GetByIdAsync_WhenDisasterCardExists_ReturnsDisasterCard()
        {
            // Arrange
            var id = 4;

            // Act
            var result = await _client.GetByIdAsync(id);

            // Arrange
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.IsType<DisasterCardDto>(result.Data);
            Assert.NotNull(result.Data);
            Assert.Null(result.ErrorMessage);

            var expected = TestDataLoader.LoadJsonFromFile<List<DisasterCardDto>>("DisasterCardDtos.json").FirstOrDefault(x => x.Id == id)
                ?? throw new InvalidOperationException($"Expected disaster card with ID {id} not found in expected data.");

            DisasterCardDtoAssertions.AssertDisasterCardDtoEqual(expected, result.Data);
        }

        [Fact]
        public async Task GetByIdAsync_WhenDisasterCardDoesNotExist_ReturnsNotFound()
        {
            // Arrange
            var id = 999; // Assuming this ID does not exist

            // Act
            var result = await _client.GetByIdAsync(id);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
        }
    }
}
