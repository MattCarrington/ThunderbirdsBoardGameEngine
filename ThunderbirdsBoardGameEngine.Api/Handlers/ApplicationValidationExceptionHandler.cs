using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class ApplicationValidationExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<ApplicationValidationExceptionHandler> _logger;

        public ApplicationValidationExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<ApplicationValidationExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApplicationValidationException validationException)
            {
                // Convert errors to ModelStateDictionary
                var modelState = new ModelStateDictionary();
                var errors = validationException.Errors ?? new Dictionary<string, string[]> { ["_"] = new[] { "An internal validation error occurred." } };
                var errorCount = errors.Sum(kvp => kvp.Value?.Length ?? 0);

                foreach (var kvp in errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        modelState.AddModelError(kvp.Key, error);
                    }
                }

                _logger.LogError(
                    validationException,
                    "Application validation failed for request {Method} {Path}. ErrorCount = {ErrorCount}, TraceId = {TraceId}",
                    httpContext.Request.Method,
                    httpContext.Request.Path.Value,
                    errorCount,
                    httpContext.TraceIdentifier);

                var validationProblemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                    httpContext,
                    modelState,
                    StatusCodes.Status500InternalServerError,
                    "Application validation failed",
                    ProblemTypes.Validation,
                    "The application encountered an internal validation error. Please contact the system administrator.");

                httpContext.Response.StatusCode = validationProblemDetails.Status ?? StatusCodes.Status500InternalServerError;

                var problemDetailsContext = new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = validationProblemDetails,
                    Exception = validationException
                };

                await _problemDetailsService.WriteAsync(problemDetailsContext);

                return true;
            }

            return false;
        }
    }
}
