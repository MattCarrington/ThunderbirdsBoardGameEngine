using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ThunderbirdsBoardGameEngine.Api.Composition;

public static class StatusCodeProblemDetailsExtensions
{
    // Services: ProblemDetails + (optional) model validation PD + your custom fields
    public static IServiceCollection AddApiProblemDetails(this IServiceCollection services)
    {
        services.AddProblemDetails(options =>
        {
            // runs for ANY ProblemDetails (incl. API versioning errors)
            options.CustomizeProblemDetails = ctx =>
            {
                var http = ctx.HttpContext;
                var pd = ctx.ProblemDetails;

                // ensure common fields
                pd.Instance ??= http.Request.Path;
                pd.Extensions["traceId"] = Activity.Current?.Id ?? http.TraceIdentifier;

                // if you have a docs catalog, set a stable type
                if (string.IsNullOrEmpty(pd.Type) && !string.IsNullOrEmpty(pd.Title))
                {
                    pd.Type = $"https://docs.yourapi.com/problems#{Slug(pd.Title)}";
                }
            };
        });

        // Optional: make automatic 400 from model binding return ValidationProblemDetails with PD content-type
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var pd = new ValidationProblemDetails(context.ModelState)
                {
                    Status = StatusCodes.Status400BadRequest,
                    Title = "Request validation failed."
                };

                pd.Extensions["traceId"] =
                    Activity.Current?.Id ?? context.HttpContext.TraceIdentifier;

                return new BadRequestObjectResult(pd); // content-type will be problem+json via IProblemDetailsService
            };
        });

        return services;

        static string Slug(string s) =>
            new string(s.ToLowerInvariant().Select(ch => char.IsLetterOrDigit(ch) ? ch : '-').ToArray())
                .Trim('-');
    }

    // Pipeline: let framework produce PD for exceptions + bare status codes
    public static IApplicationBuilder UseApiProblemDetails(this IApplicationBuilder app)
    {
        app.UseStatusCodePages();

        return app;
    }
}
