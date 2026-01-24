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
        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsAllDisasterCards()
        {
            // Arrange
            using var sp = CatalogClientProviderFactory.Build(CatalogTestConfig.CatalogBaseUrl);
            var client = sp.GetRequiredService<IDisasterCardsClient>();

            // Act
            var result = await client.GetAllAsync();

            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);            
            Assert.Null(result.ErrorMessage);

            var disasterCards = Assert.IsType<IReadOnlyList<DisasterCardDto>>(result.Data, exactMatch: false);
            Assert.NotNull(disasterCards);
            Assert.NotEmpty(disasterCards);
            Assert.Equal(34, disasterCards.Count);

            Assert.All(disasterCards, card =>
            {
                Assert.False(string.IsNullOrWhiteSpace(card.Name));
                Assert.False(string.IsNullOrWhiteSpace(card.Code));
                Assert.True(card.DifficultyNumber > 0);
                Assert.False(string.IsNullOrWhiteSpace(card.RescueType));
                Assert.False(string.IsNullOrWhiteSpace(card.Location));
                Assert.NotEmpty(card.BonusConditions);
                Assert.NotEmpty(card.Rewards);
            });

            Assert.Equal(
                disasterCards.Count,
                disasterCards.Select(c => c.Code).Distinct().Count()
            );

            Assert.Contains(result.Data, c => c.Code == "signal-from-sigma");
            Assert.Contains(result.Data, c => c.Code == "they-call-him-mr-x");
        }
    }
}
