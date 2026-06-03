using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class ReferenceDataNotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<ReferenceDataNotFoundExceptionHandler> _logger;

        public ReferenceDataNotFoundExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<ReferenceDataNotFoundExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ReferenceDataNotFoundException notFoundException)
            {
                _logger.LogWarning(
                    "{ResourceType} with Code {Code} not found for request {Method} {Path}. TraceId = {TraceId}",
                    notFoundException.ResourceType,
                    notFoundException.Code,
                    httpContext.Request.Method,
                    httpContext.Request.Path.Value,
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status404NotFound,
                    "Resource not found.",
                    ProblemTypes.NotFound,
                    "The requested resource could not be found.");

                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status404NotFound;

                var problemDetailsContext = new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails,
                    Exception = exception
                };

                await _problemDetailsService.WriteAsync(problemDetailsContext);

                return true;
            }

            return false;
        }
    }
}
