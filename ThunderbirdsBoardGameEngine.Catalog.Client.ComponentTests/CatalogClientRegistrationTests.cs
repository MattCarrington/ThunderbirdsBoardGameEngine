using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Factories;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.ComponentTests
{
    public class CatalogClientRegistrationTests
    {
        [Fact]
        public async Task AddCatalogClients_WhenInvalidBaseAddress_ThrowsException()
        {
            // Arrange
            await using var provider = CatalogClientProviderFactory.Build("http:/example.com");

            // Act & Assert
            Assert.Throws<OptionsValidationException>(
                () => provider.GetRequiredService<IOptions<CatalogClientOptions>>().Value);
        }

        [Theory]
        [MemberData(nameof(Clients))]
        public async Task AddCatalogClients_Registers_TypedClient(Type service, Type impl)
        {
            await using var sp = CatalogClientProviderFactory.Build("https://example.com/");
            var instance = sp.GetRequiredService(service);
            Assert.Equal(impl, instance.GetType());
        }

        public static IEnumerable<object[]> Clients()
        {
            return
            [
                [typeof(IDisasterCardsClient), typeof(Clients.V1.DisasterCardsClient)]
            ];
        }
    }
}
