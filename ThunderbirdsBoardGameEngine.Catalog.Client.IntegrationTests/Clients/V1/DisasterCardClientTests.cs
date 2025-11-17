using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.IntegrationTests.Clients.V1
{
    public class DisasterCardClientTests
    {
        private readonly IDisasterCardsClient _client;
        
        public DisasterCardClientTests()
        {
            var sp = CatalogClientProviderFactory.Build("http://localhost:8080");
            _client = sp.GetRequiredService<IDisasterCardsClient>();
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

            var expected = await TestDataLoader.LoadJsonFromFileAsync<List<DisasterCardDto>>(DisasterCardTestFileCatalog.DataOnly("disaster-card-dtos.json"))
                ?? throw new InvalidOperationException("Failed to load expected data");

            DisasterCardDtoAssertions.AssertOrderInsensitive(expected, result.Data.ToList());
        }
    }
}
