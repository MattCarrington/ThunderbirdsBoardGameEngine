using Microsoft.AspNetCore.Hosting;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories
{
    public class RateLimitingWebApplicationFactory : ApiComponentWebApplicationFactory
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            base.ConfigureWebHost(builder);

            builder.UseSetting("RateLimiting:PublicApi:PermitLimit", "2");
            builder.UseSetting("RateLimiting:PublicApi:WindowSeconds", "10");
            builder.UseSetting("RateLimiting:PublicApi:QueueLimit", "0");
        }
    }
}
