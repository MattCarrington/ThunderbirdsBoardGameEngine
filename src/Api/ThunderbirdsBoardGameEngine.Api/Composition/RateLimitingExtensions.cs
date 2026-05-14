using Microsoft.AspNetCore.RateLimiting;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddAppRateLimiting(
            this IServiceCollection services,
            IConfiguration configuration)
        {
            var rateLimitOptions = configuration
                .GetSection("RateLimiting:PublicApi")
                .Get<PublicApiRateLimitOptions>()
                ?? new PublicApiRateLimitOptions();

            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("public-api", limiterOptions =>
                {
                    limiterOptions.Window = TimeSpan.FromSeconds(rateLimitOptions.WindowSeconds);
                    limiterOptions.PermitLimit = rateLimitOptions.PermitLimit;
                    limiterOptions.QueueLimit = rateLimitOptions.QueueLimit;
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }

        public static IApplicationBuilder UseAppRateLimiting(this IApplicationBuilder app)
        {
            return app.UseRateLimiter();
        }
    }
}