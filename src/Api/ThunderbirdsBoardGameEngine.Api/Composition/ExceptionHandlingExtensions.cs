using ThunderbirdsBoardGameEngine.Api.Handlers;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class ExceptionHandlingExtensions
    {
        public static IServiceCollection AddApiExceptionHandling(this IServiceCollection services)
        {
            // Order: specific → fallback
            services.AddExceptionHandler<ReferenceDataNotFoundExceptionHandler>();
            services.AddExceptionHandler<BadRequestExceptionHandler>();
            services.AddExceptionHandler<UnhandledExceptionHandler>();

            return services;
        }

        public static IApplicationBuilder UseApiExceptionHandling(this IApplicationBuilder app)
        {
            app.UseExceptionHandler();
            return app;
        }
    }
}
