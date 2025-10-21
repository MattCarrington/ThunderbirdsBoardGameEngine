using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Diagnostics;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes
{
    public sealed class FakeProblemDetailsFactory : ProblemDetailsFactory
    {
        public override ProblemDetails CreateProblemDetails(HttpContext httpContext, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            var status = statusCode ?? StatusCodes.Status500InternalServerError;

            var problemDetails = new ProblemDetails
            {
                Status = status,
                Title = title ?? TitleFor(status),
                Type = type ?? "about:blank",
                Detail = detail,
                Instance = instance ?? httpContext.Request.Path
            };

            AddTraceId(problemDetails, httpContext);

            return problemDetails;
        }

        public override ValidationProblemDetails CreateValidationProblemDetails(HttpContext httpContext, ModelStateDictionary modelStateDictionary, int? statusCode = null, string? title = null, string? type = null, string? detail = null, string? instance = null)
        {
            var status = statusCode ?? StatusCodes.Status400BadRequest;

            var validationProblemDetails = new ValidationProblemDetails(modelStateDictionary)
            {
                Status = status,
                Title = title ?? TitleFor(status),
                Type = type ?? "about:blank",
                Detail = detail,
                Instance = instance ?? httpContext.Request.Path
            };

            AddTraceId(validationProblemDetails, httpContext);

            return validationProblemDetails;
        }

        private static string TitleFor(int? status)
        {
            return status switch
            {
                StatusCodes.Status400BadRequest => "Bad Request",
                StatusCodes.Status401Unauthorized => "Unauthorized",
                StatusCodes.Status403Forbidden => "Forbidden",
                StatusCodes.Status404NotFound => "Resource not found",
                StatusCodes.Status405MethodNotAllowed => "Method not allowed",
                StatusCodes.Status409Conflict => "Conflict",
                StatusCodes.Status422UnprocessableEntity => "Unprocessable Entity",
                StatusCodes.Status500InternalServerError => "Internal Server Error",
                StatusCodes.Status503ServiceUnavailable => "Service Unavailable",
                _ => "Request failed"
            };
        }

        private static void AddTraceId(ProblemDetails problemDetails, HttpContext httpContext)
        {
            var traceId = httpContext.TraceIdentifier ?? Activity.Current?.Id;

            if (!string.IsNullOrEmpty(traceId))
            {
                problemDetails.Extensions["traceId"] = traceId;
            }
        }
    }
}
