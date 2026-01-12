using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;

namespace ThunderbirdsBoardGameEngine.TestUtils.Rules.Factories
{
    public static class RulesClientProviderFactory
    {
        public static ServiceProvider Build(string baseAddress, Action<IServiceCollection>? overrides = null)
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RulesClient:BaseAddress"] = baseAddress
                })
                .Build();

            var services = new ServiceCollection();
            services.AddRulesClients(cfg);   // your production registration
            overrides?.Invoke(services);       // optional: test-time tweaks

            return services.BuildServiceProvider();
        }
    }
}
