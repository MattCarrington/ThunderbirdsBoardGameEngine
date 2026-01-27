using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class DomainValidationExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<DomainValidationExceptionHandler> _logger;

        public DomainValidationExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<DomainValidationExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is DomainValidationException)
            {
                _logger.LogError(
                    exception,
                    "Catalog validation failed for request {Method} {Path}. TraceId = {TraceId}",
                    httpContext.Request.Method,
                    httpContext.Request.Path.Value,
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    "Catalog configuration error",
                    ProblemTypes.Validation,
                    "The catalog contains invalid configuration. Please contact the system administrator.");

                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

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
