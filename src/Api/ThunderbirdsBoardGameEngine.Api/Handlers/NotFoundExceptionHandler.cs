using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class NotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<NotFoundExceptionHandler> _logger;

        public NotFoundExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService, ILogger<NotFoundExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is NotFoundException notFoundException)
            {
                _logger.LogWarning(
                    "{ResourceType} with ID {ResourceId} not found for request {Method} {Path}. TraceId = {TraceId}",
                    notFoundException.ResourceType,
                    notFoundException.ResourceId,
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
