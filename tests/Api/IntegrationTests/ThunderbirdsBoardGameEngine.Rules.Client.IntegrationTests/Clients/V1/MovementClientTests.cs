using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.IntegrationTests.Clients.V1
{
    public class MovementClientTests
    {
        [Fact]
        public async Task ValidateMovementAsyncReturnsResultAsync()
        {
            // Arrange
            using var sp = RulesClientProviderFactory.Build(RulesTestConfig.RulesBaseUrl);
            var client = sp.GetRequiredService<IMovementClient>();

            var request = new ValidateMovementRequestDto
            {
                StartLocation = "south-america",
                DestinationLocation = "asia"
            };

            var thunderbird = "thunderbird-1";

            // Act
            var response = await client.ValidateMovementAsync(thunderbird, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.ErrorMessage);

            var result = Assert.IsType<ValidateMovementResponseDto>(response.Data);
            Assert.NotNull(result);
            Assert.True(result.IsValid);
            Assert.Equal(3, result.SpacesTravelled);
            Assert.Equal(1, result.ActionPointCost);
            Assert.Equal(3, result.TopSpeed);
            Assert.NotEmpty(result.Route);
            Assert.Equal("south-america", result.Route.First());
            Assert.Equal("asia", result.Route.Last());
            Assert.Empty(result.Messages);
        }
    }
}
