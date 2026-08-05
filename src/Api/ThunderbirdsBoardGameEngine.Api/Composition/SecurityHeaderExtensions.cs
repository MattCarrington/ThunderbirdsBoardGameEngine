namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class SecurityHeaderExtensions
    {
        public static IApplicationBuilder UseCapabilitySecurityHeaders(this IApplicationBuilder app)
        {
            return app.Use(async (context, next) =>
            {
                context.Response.OnStarting(() =>
                {
                    context.Response.Headers["Referrer-Policy"] = "no-referrer";

                    if (IsGameRequest(context.Request.Path))
                    {
                        context.Response.Headers.CacheControl = "no-store";
                    }

                    return Task.CompletedTask;
                });

                await next();
            });
        }

        private static bool IsGameRequest(PathString path) =>
            path.StartsWithSegments("/api/games")
            || path.StartsWithSegments("/games");
    }
}
