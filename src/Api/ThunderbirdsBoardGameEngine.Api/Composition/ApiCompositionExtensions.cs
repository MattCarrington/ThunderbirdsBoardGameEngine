namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class ApiCompositionExtensions
    {
        public static IServiceCollection AddApiServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHeaderApiVersioning();
            services.AddApiProblemDetails();
            services.AddApiExceptionHandling();

#pragma warning disable CS0618 // Type or member is obsolete
            services.AddCatalogHealthChecks();
#pragma warning restore CS0618

            services.AddApiCors(configuration);
            return services;
        }
    }
}
