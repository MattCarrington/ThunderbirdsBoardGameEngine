using Microsoft.Extensions.DependencyInjection;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.IntegrationTests.Clients.V1
{
    public class CharactersClientTests
    {
        [Fact]
        public async Task GetAllAsync_WhenCalled_ReturnsAllCharacters()
        {
            // Arrange
            using var sp = CatalogClientProviderFactory.Build(CatalogTestConfig.CatalogBaseUrl);

            var client = sp.GetRequiredService<ICharactersClient>();

            // Act
            var result = await client.GetAllAsync();

            // Assert
            var expected = new List<CharacterDto>
            {
                new() { Key = "scott", DisplayName = "Scott" },
                new() { Key = "virgil", DisplayName = "Virgil" },
                new() { Key = "alan", DisplayName = "Alan" },
                new() { Key = "gordon", DisplayName = "Gordon" },
                new() { Key = "john", DisplayName = "John" },
                new() { Key = "lady-penelope", DisplayName = "Lady Penelope" }
            };

            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.ErrorMessage);

            var data = Assert.IsType<IReadOnlyList<CharacterDto>>(result.Data, exactMatch: false);
            Assert.NotNull(data);
            Assert.NotEmpty(data);
            Assert.Equal(expected.Count, data.Count);
            Assert.Equal(expected.OrderBy(x => x.Key), data.OrderBy(x => x.Key));
        }
    }
}
