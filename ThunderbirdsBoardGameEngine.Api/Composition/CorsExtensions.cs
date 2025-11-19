namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class CorsExtensions
    {
        public const string ApiCorsPolicyName = "CorsPolicy";

        public static IServiceCollection AddApiCors(this IServiceCollection services, IConfiguration configuration)
        {
            var allowedOrigins = configuration.GetSection("Cors:AllowedOrigins").Get<string[]>() ?? [];

            services.AddCors(options =>
            {
                options.AddPolicy(ApiCorsPolicyName, policy =>
                {
                    if (allowedOrigins.Length > 0)
                    {
                        policy.WithOrigins(allowedOrigins)
                              .AllowAnyHeader()
                              .AllowAnyMethod();
                        // .AllowCredentials(); // only if you actually need cookies/auth from browser
                    }
                    else
                    {
                        // Optional: dev-only fallback
                        policy
                            .AllowAnyOrigin()
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    }
                });
            });

            return services;
        }

        public static IApplicationBuilder UseApiCors(this IApplicationBuilder app)
        {
            return app.UseCors(ApiCorsPolicyName);
        }
    }
}
