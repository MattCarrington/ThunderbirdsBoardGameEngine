using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.GameState.V1
{
    public class GetGameTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _httpClient;
        private readonly IGameRepository _repository;

        private const int ApiVersion = 1;

        public GetGameTests(ApiComponentWebApplicationFactory factory)
        {
            _httpClient = factory.CreateClient();
            _repository = factory.Repository;
        }

        [Fact]
        public async Task CanGetExistingGame()
        {
            // Arrange
            var gameId = Guid.NewGuid();

            var game = new StandardGameSetupFactory().Create(gameId, DateTimeOffset.UtcNow);

            await _repository.CreateNewGameSession(game, TestContext.Current.CancellationToken);

            var route = $"/api/games/{gameId}";

            using var request = new HttpRequestMessage(HttpMethod.Get, route);
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _httpClient.SendAsync(request, TestContext.Current.CancellationToken);

            // Arrange
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<GameStateResponseDto>(cancellationToken: TestContext.Current.CancellationToken);
            Assert.NotNull(result);
            Assert.NotNull(result.ThunderbirdMachines);
        }
    }
}
