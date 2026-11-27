using Microsoft.AspNetCore.Http;
using System.Net;
using System.Net.Http.Json;
using ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Rescue.DetermineFabCardBonus.V1;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Endpoints.Rules.V1
{
    public class DetermineFabCardBonusTests : IClassFixture<ApiComponentWebApplicationFactory>
    {
        private readonly HttpClient _client;

        private const int ApiVersion = 1;

        public DetermineFabCardBonusTests(ApiComponentWebApplicationFactory factory)
        {
            _client = factory.CreateClient();
        }

        [Theory]
        [InlineData("ricochet", "astronaut-spacewalk")]
        [InlineData("desperate-intruder", "underwater-sealing-unit")]
        public async Task ReturnsSuccessWhenRequestIsValid(string disasterCardCode, string expectedFabCardCode)
        {
            // Arrange
            using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/rules/rescue/{disasterCardCode}/fab-card-bonus");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

            var result = await response.Content.ReadFromJsonAsync<FabCardBonusResponseDto>(cancellationToken: TestContext.Current.CancellationToken);

            var expectedBonus = 3;

            Assert.NotNull(result);
            Assert.Equal(expectedFabCardCode, result.FabCardCode);
            Assert.Equal(expectedBonus, result.BonusValue);
        }

        [Fact]
        public async Task ReturnsNotFoundWhenThunderbirdDoesNotExist()
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, "/api/rules/rescue/non-existant-disaster-card/fab-card-bonus");
            request.Headers.Add("X-API-Version", ApiVersion.ToString());

            // Act
            using var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

            // Assert
            await ProblemDetailsAssertions.AssertNotFoundAsync(response, "Resource not found.");
        }
    }
}
