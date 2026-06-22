using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ThunderbirdsBoardGameEngine.Rules.Client.Extensions;

namespace ThunderbirdsBoardGameEngine.SmokeTests.Factories
{
    public static class RulesClientProviderFactory
    {
        public static ServiceProvider Build(string baseAddress)
        {
            var cfg = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string?>
                {
                    ["RulesClient:BaseAddress"] = baseAddress
                })
                .Build();

            var services = new ServiceCollection();
            services.AddRulesClients(cfg);

            return services.BuildServiceProvider();
        }
    }
}
