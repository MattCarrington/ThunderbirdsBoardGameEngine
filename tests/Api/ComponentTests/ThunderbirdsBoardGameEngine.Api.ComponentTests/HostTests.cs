using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests
{
    public class HostTests : IClassFixture<CustomWebApplicationFactory>
    {
        private readonly CustomWebApplicationFactory _factory;

        public HostTests(CustomWebApplicationFactory factory)
        {
            _factory = factory;
        }

        [Fact]
        public void Build_WhenMissingFilePath_FailFast()
        {
            // Arrange
            var faultyFactory = _factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureAppConfiguration((context, configBuilder) =>
                {
                    configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                    {
                        { "Catalog:DisasterCards:Json:FilePath", string.Empty }
                    });
                });
            });

            // Act & Assert
            var exception = Assert.Throws<OptionsValidationException>(() => faultyFactory.CreateClient());
            Assert.Contains("Catalog:DisasterCards:Json:FilePath is required.", exception.Message);
        }
    }
}
