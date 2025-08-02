using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;
using ThunderbirdsBoardGameEngine.TestUtils.Helpers;

namespace ThunderbirdsBoardGameEngine.GameData.Api.IntegrationTests
{
    public class CustomWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            var filepath = TestDataPathHelper.GetPath("DisasterCards.json");

            builder.ConfigureAppConfiguration((context, configBuilder) =>
            {
                configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
                {
                    { "CardData:DisasterCardsFilePath", filepath }
                });
            });
        }
    }
}
