using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace ThunderbirdsBoardGameEngine.Api.Composition
{
    public static class StatusCodeProblemDetailsExtensions
    {
        public static IApplicationBuilder UseStatusCodeProblemDetails(this IApplicationBuilder app)
        {
            return app.UseStatusCodePages(async context =>
            {
                var http = context.HttpContext;

                // only act on error codes with empty body
                if (http.Response.StatusCode < 400 || http.Response.HasStarted) 
                { 
                    return; 
                }

                var pds = http.RequestServices.GetRequiredService<IProblemDetailsService>();

                var (title, type) = http.Response.StatusCode switch
                {
                    StatusCodes.Status401Unauthorized => ("Unauthorized", "about:blank"),
                    StatusCodes.Status403Forbidden => ("Forbidden", "about:blank"),
                    StatusCodes.Status404NotFound => ("Resource not found", "about:blank"),
                    StatusCodes.Status405MethodNotAllowed => ("Method not allowed", "about:blank"),
                    StatusCodes.Status406NotAcceptable => ("Not acceptable", "about:blank"),
                    StatusCodes.Status415UnsupportedMediaType => ("Unsupported media type", "about:blank"),
                    StatusCodes.Status422UnprocessableEntity => ("Unprocessable entity", "about:blank"),
                    StatusCodes.Status429TooManyRequests => ("Too many requests", "about:blank"),
                    _ => ("Request failed", "about:blank")
                };

                var problem = new ProblemDetails
                {
                    Status = http.Response.StatusCode,
                    Title = title,
                    Type = type,
                    Instance = http.Request.Path
                };

                await pds.WriteAsync(new ProblemDetailsContext
                {
                    HttpContext = http,
                    ProblemDetails = problem
                });
            });
        }

        public static IServiceCollection AddModelStateProblemDetails(this IServiceCollection services)
        {
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

                    return new BadRequestObjectResult(pd)
                    {
                        ContentTypes = { "application/problem+json" }
                    };
                };
            });

            return services;
        }
    }
}
