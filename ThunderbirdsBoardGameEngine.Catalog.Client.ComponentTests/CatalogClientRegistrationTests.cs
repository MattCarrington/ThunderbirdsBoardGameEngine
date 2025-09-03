using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Http;
using Microsoft.Extensions.Options;
using System.Net;
using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.TestUtils.Factories;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.ComponentTests
{
    public class CatalogClientRegistrationTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        [InlineData("localhost")]           // missing scheme
        [InlineData("/api")]                // relative
        [InlineData("http:/example.com")]   // malformed
        public async Task AddCatalogClients_WhenInvalidBaseAddress_ThrowsException(string baseAddress)
        {
            // Arrange
            await using var provider = CatalogClientProviderFactory.Build(baseAddress);

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
