using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.Configuration;

namespace ThunderbirdsBoardGameEngine.Api.ComponentTests.Factories
{
    public class RateLimitingWebApplicationFactory : WebApplicationFactory<Program>
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseSetting("RateLimiting:PublicApi:PermitLimit", "2");
            builder.UseSetting("RateLimiting:PublicApi:WindowSeconds", "10");
            builder.UseSetting("RateLimiting:PublicApi:QueueLimit", "0");
        }
    }
}
