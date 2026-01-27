using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Extensions;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;

namespace ThunderbirdsBoardGameEngine.Catalog.ComponentTests.Fixtures
{
    public class InfrastructureRegistrationFixture : IDisposable
    {
        private readonly ServiceProvider _serviceProvider;

        public InfrastructureRegistrationFixture()
        {
            var services = new ServiceCollection();
            services.AddLogging();

            services.AddSingleton<IHostEnvironment>(
                StubHostEnvironment.WithNullProvider(Directory.GetCurrentDirectory()));

            // Register other services needed for testing
            _serviceProvider = services.BuildServiceProvider();
        }

        public ServiceProvider Build<TOptions>(string configurationKey, string absolutePath) where TOptions : class
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    [configurationKey] = absolutePath
                })
                .Build();

            var services = new ServiceCollection();
            services.AddLogging();
            services.AddSingleton(_serviceProvider.GetRequiredService<IHostEnvironment>());

            services.AddCatalogInfrastructure(cfg);

            var sp = services.BuildServiceProvider(new ServiceProviderOptions { ValidateScopes = true });

            _ = sp.GetRequiredService<IOptions<TOptions>>().Value;

            return sp;
        }

        public void Dispose()
        {
            _serviceProvider?.Dispose();
        }
    }
}
