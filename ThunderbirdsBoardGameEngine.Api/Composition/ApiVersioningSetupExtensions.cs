using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

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

            // Emit RFC7807 problem+json responses for versioning errors
            options.ErrorResponses = new ProblemDetailsErrorResponseProvider();

            options.ApiVersionReader = new HeaderApiVersionReader("X-API-Version");
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";         // v1, v2, etc.
            options.SubstituteApiVersionInUrl = false;  // you're not using URL segments
        });

        return services;
    }
}
