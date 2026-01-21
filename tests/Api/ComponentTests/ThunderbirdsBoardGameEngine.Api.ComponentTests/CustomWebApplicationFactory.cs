using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.TestFileCatalogs;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var disasterCardsFilePath = DisasterCardTestFileCatalog.Enveloped("disaster-cards.json");
            var charactersFilePath = CharacterDefinitionTestFileCatalog.Data("characters.json");

            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "Catalog:DisasterCards:Json:FilePath", disasterCardsFilePath },
                    {  "Catalog:Characters:Json:FilePath", charactersFilePath   }
                });
            });
        }
    }
}
