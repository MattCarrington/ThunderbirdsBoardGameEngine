using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var filepath = TestDataPathHelper.GetPath(DisasterCardTestFileCatalog.Enveloped("disaster-cards.json"));

            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Catalog:DisasterCards:Json:FilePath", filepath },
                    { "Catalog:DisasterCards:Warmup:Enabled", "false" },
                    { "Catalog:DisasterCards:Warmup:Timeout", "00:00:01" }
                });
            });
        }
    }
}
