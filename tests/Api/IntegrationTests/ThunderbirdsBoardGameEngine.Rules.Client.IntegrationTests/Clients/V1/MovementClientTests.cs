using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.AccessibleLocations.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Rules.Client.IntegrationTests.Clients.V1
{
    public class MovementClientTests
    {
        [Fact]
        public async Task ValidateMovementAsyncReturnsResult()
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
            Assert.Equal(3, result.ThunderbirdTopSpeed);
            Assert.Equal(3, result.EffectiveTopSpeed);
            Assert.NotEmpty(result.Route);
            Assert.Equal("south-america", result.Route.First());
            Assert.Equal("asia", result.Route.Last());
            Assert.Empty(result.Messages);
        }

        [Fact]
        public async Task ValidateMovementAsyncReturnsNotFoundWhenThunderbirdDoesNotExist()
        {
            // Arrange
            using var sp = RulesClientProviderFactory.Build(RulesTestConfig.RulesBaseUrl);
            var client = sp.GetRequiredService<IMovementClient>();

            var request = new ValidateMovementRequestDto
            {
                StartLocation = "south-america",
                DestinationLocation = "south-pacific"
            };

            var thunderbird = "thunderbird-x";

            // Act
            var response = await client.ValidateMovementAsync(thunderbird, request, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(response.ErrorMessage);
            Assert.Null(response.Data);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsyncReturnsResult()
        {
            // Arrange
            using var sp = RulesClientProviderFactory.Build(RulesTestConfig.RulesBaseUrl);
            var client = sp.GetRequiredService<IMovementClient>();

            var thunderbird = "thunderbird-3";

            // Act
            var response = await client.GetAccessibleLocationsAsync(thunderbird, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.True(response.Success);
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Null(response.ErrorMessage);
            var result = Assert.IsType<AccessibleLocationsResponseDto>(response.Data);
            Assert.NotNull(result);
            Assert.NotEmpty(result.AccessibleLocations);
        }

        [Fact]
        public async Task GetAccessibleLocationsAsyncReturnsNotFoundWhenThunderbirdDoesNotExist()
        {
            // Arrange
            using var sp = RulesClientProviderFactory.Build(RulesTestConfig.RulesBaseUrl);
            var client = sp.GetRequiredService<IMovementClient>();

            var thunderbird = "thunderbird-x";

            // Act
            var response = await client.GetAccessibleLocationsAsync(thunderbird, TestContext.Current.CancellationToken);

            // Assert
            Assert.NotNull(response);
            Assert.False(response.Success);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
            Assert.NotNull(response.ErrorMessage);
            Assert.Null(response.Data);
        }
    }
}
