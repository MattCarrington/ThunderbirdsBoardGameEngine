using FluentAssertions;
using System.Net;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.GameData.Api.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Assertions;
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
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data.Should().NotBeEmpty();
            result.Data.Should().BeAssignableTo<IReadOnlyList<DisasterCardDto>>();
            result.ErrorMessage.Should().BeNullOrWhiteSpace();

            var expected = GetExpectedDisasterCardDtos();

            result.Data.Count.Should().Be(expected.Count);

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
            result.Should().NotBeNull();
            result.Success.Should().BeTrue();
            result.StatusCode.Should().Be(HttpStatusCode.OK);
            result.Data.Should().NotBeNull();
            result.Data.Should().BeAssignableTo<DisasterCardDto>();
            result.ErrorMessage.Should().BeNullOrWhiteSpace();

            var expected = GetExpectedDisasterCardDtos().FirstOrDefault(dc => dc.Id == id)
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
            result.Should().NotBeNull();
            result.Success.Should().BeFalse();
            result.StatusCode.Should().Be(HttpStatusCode.NotFound);
            result.Data.Should().BeNull();
            result.ErrorMessage.Should().NotBeNullOrWhiteSpace();
        }

        private static List<DisasterCardDto> GetExpectedDisasterCardDtos()
        {
            var json = File.ReadAllText("ExpectedData/DisasterCardDtos.json");
            return JsonSerializer.Deserialize<List<DisasterCardDto>>(json, _jsonOptions)
                ?? throw new InvalidOperationException("Failed to deserialize expected disaster card DTOs.");
        }
    }
}
