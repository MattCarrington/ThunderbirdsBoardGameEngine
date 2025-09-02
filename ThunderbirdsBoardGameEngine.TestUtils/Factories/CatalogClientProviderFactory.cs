using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using ThunderbirdsBoardGameEngine.Catalog.Client.Extensions;

namespace ThunderbirdsBoardGameEngine.TestUtils.Factories
{
    public static class CatalogClientProviderFactory
    {
        public static ServiceProvider Build(string baseAddress, Action<IServiceCollection>? overrides = null)
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["CatalogClient:BaseAddress"] = baseAddress
                })
                .Build();

            var services = new ServiceCollection();
            services.AddCatalogClients(cfg);   // your production registration
            overrides?.Invoke(services);       // optional: test-time tweaks

            return services.BuildServiceProvider();
        }
    }
}
