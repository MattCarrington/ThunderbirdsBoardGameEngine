using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyModel;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1.ThunderbirdMachines;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState.V1
{
    public class MoveThunderbirdTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly IGameRepository _repository;

        private const int ApiVersion = 1;

        public MoveThunderbirdTests(ApiComponentWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _repository = factory.Repository;
        }

        [Fact]
        public async Task ThunderbirdCanMove()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-sun"
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.NotNull(result.ThunderbirdMachines);

            var thunderbird3 = result.ThunderbirdMachines.SingleOrDefault(tm => tm.ThunderbirdCode == thunderbirdCode.Value);
            Assert.NotNull(thunderbird3);
            Assert.Equal("the-sun", thunderbird3.LocationCode);
        }

        [Fact]
        public async Task MoveNonExistingThunderbirdReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = "non-existing-thunderbird";

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-sun"
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Thunderbird Machine was not found.");
        }

        [Fact]
        public async Task MoveThunderbirdToInvalidLocationReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "invalid-location"
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }

        [Fact]
        public async Task MoveThunderbirdToUnprocessableLocationReturnsUnprocessableEntity()
        {
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird5;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-moon"
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.UnprocessableEntity, response.StatusCode);
        }

        [Fact]
        public async Task MoveThunderbirdInInvalidGameStateReturnsInternalServerError()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirds = new Dictionary<ThunderbirdCode, LocationCode>
            {
                { KnownThunderbirdCodes.Thunderbird1, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird2, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird3, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird4, new("the-moon") },
                { KnownThunderbirdCodes.Thunderbird5, new("the-moon") }
            };

            var characters = new Dictionary<CharacterCode, ThunderbirdCode>
            {
                { KnownCharacterCodes.Scott, KnownThunderbirdCodes.Thunderbird1 },
                { KnownCharacterCodes.Virgil, KnownThunderbirdCodes.Thunderbird2 },
                { KnownCharacterCodes.Alan, KnownThunderbirdCodes.Thunderbird3 },
                { KnownCharacterCodes.Gordon, KnownThunderbirdCodes.Thunderbird4 },
                { KnownCharacterCodes.John, KnownThunderbirdCodes.Thunderbird5 }
            };

            var game = Game.Create(gameId, DateTimeOffset.UtcNow, "invalid-setup", thunderbirds, characters);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-sun"
            };
            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.InternalServerError, response.StatusCode);
        }

        [Fact]
        public async Task MoveThunderbirdInNonExistingGameReturnsNotFound()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-sun"
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Game not found.");
        }

        [Fact]
        public async Task MissingDestinationInRequestDtoReturnsBadRequest()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new { };  // Empty anonymous type to simulate missing destination

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task NullDestinationInRequestDtoReturnsBadRequest()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new
            {
                destination = (string?)null
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Theory]
        [ClassData(typeof(WhiteSpaceStringData))]
        public async Task WhiteSpaceDestinationInRequestDtoReturnsBadRequest(string destination)
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = destination
            };

            var route = $"/api/games/{gameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task InvalidGuidReturnsNotFound()
        {
            // Arrange
            var invalidGameId = "not-a-guid";

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-sun"
            };

            var route = $"/api/games/{invalidGameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Not Found");
        }

        [Fact]
        public async Task EmptyGuidReturnsBadRequest()
        {
            // Arrange
            var emptyGameId = Guid.Empty;

            var thunderbirdCode = KnownThunderbirdCodes.Thunderbird3;

            var dto = new MoveThunderbirdMachineRequestDto
            {
                Destination = "the-sun"
            };

            var route = $"/api/games/{emptyGameId}/thunderbird-machines/{thunderbirdCode.Value}/move";

            using var request = new HttpRequestMessage(HttpMethod.Post, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());
            request.Content = JsonContent.Create(dto);

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            var details = await ProblemDetailsAssertions.AssertBadRequestAsync(response, "Bad request.");

            Assert.Equal("The provided GUID is empty.", details.Detail);
        }
    }
}
