using Microsoft.AspNetCore.RateLimiting;
using System.Runtime.CompilerServices;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class RateLimitingExtensions
    {
        public static IServiceCollection AddAppRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddFixedWindowLimiter("public-api", limiterOptions =>
                {
                    limiterOptions.Window = TimeSpan.FromMinutes(1);
                    limiterOptions.PermitLimit = 60;
                    limiterOptions.QueueLimit = 0; // no queuing, just reject when limit is hit
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
