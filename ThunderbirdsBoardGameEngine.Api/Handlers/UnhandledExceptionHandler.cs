using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public class UnhandledExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<UnhandledExceptionHandler> _logger;

        public UnhandledExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService, ILogger<UnhandledExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            _logger.LogError(
                exception,
                "Unhandled exception occurred for request {Method} {Path}. TraceId = {TraceId}",
                httpContext.Request.Method,
                httpContext.Request.Path.Value,
                httpContext.TraceIdentifier);

            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                ProblemTypes.Unexpected);

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
    }
}
