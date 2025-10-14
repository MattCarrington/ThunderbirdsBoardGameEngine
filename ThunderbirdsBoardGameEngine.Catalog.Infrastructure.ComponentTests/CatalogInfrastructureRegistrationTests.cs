using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ComponentTests
{
    public class CatalogInfrastructureRegistrationTests
    {
        [Fact]
        public void AddInfrastructure_WhenBadFilePath_FailsFast()
        {
            // Arrange: obviously invalid path so validator will fail
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["Catalog:DisasterCards:Json:FilePath"] = "/no/such/dir/cards.json"
                })
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton<IHostEnvironment>(
                StubHostEnvironment.WithNullProvider(Directory.GetCurrentDirectory()));

            services.AddInfrastructure(cfg);

            using var sp = services.BuildServiceProvider(
                new ServiceProviderOptions { ValidateScopes = true });

            // Act + Assert: accessing .Value triggers Bind + PostConfigure + Validate
            var ex = Assert.Throws<OptionsValidationException>(
                () => sp.GetRequiredService<IOptions<DisasterCardJsonOptions>>().Value);

            Assert.Contains("Catalog:DisasterCards:Json:FilePath", ex.Message, StringComparison.OrdinalIgnoreCase);
        }
    }
}
