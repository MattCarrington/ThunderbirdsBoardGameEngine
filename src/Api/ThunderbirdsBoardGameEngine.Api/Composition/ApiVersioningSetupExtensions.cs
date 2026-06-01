using Asp.Versioning;

namespace ThunderbirdsBoardGameEngine.Api.Composition;

public static class ApiVersioningSetupExtensions
{
    public static IServiceCollection AddHeaderApiVersioning(this IServiceCollection services)
    {
        services.AddApiVersioning(options =>
        {
            options.AssumeDefaultVersionWhenUnspecified = false;   // keep clients explicit
            options.DefaultApiVersion = new ApiVersion(1, 0);      // not used unless flag above is true
            options.ReportApiVersions = true;                      // adds api-supported-versions headers
            options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
        })
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";         // v1, v2, etc.
            options.SubstituteApiVersionInUrl = false;  // you're not using URL segments
        });

        services.AddProblemDetails();

        return services;
    }
}
