namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class ApiCompositionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHeaderApiVersioning();
            services.AddApiProblemDetails();
            services.AddApiExceptionHandling();
            services.AddApiHealthChecks();
            services.AddApiCors(configuration);
            return services;
        }
    }
}
