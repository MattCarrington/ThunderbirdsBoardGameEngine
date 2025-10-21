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

        public ApplicationValidationExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ApplicationValidationException applicationValidationException)
            {
                // Convert errors to ModelStateDictionary
                var modelState = new ModelStateDictionary();
                var errors = applicationValidationException.Errors ?? new Dictionary<string, string[]> { ["_"] = new[] { applicationValidationException.Message } };
                
                foreach (var kvp in errors)
                {
                    foreach (var error in kvp.Value)
                    {
                        modelState.AddModelError(kvp.Key, error);
                    }
                }

                var validationProblemDetails = _problemDetailsFactory.CreateValidationProblemDetails(
                    httpContext,
                    modelState,
                    StatusCodes.Status500InternalServerError,
                    "Application validation failed",
                    ProblemTypes.Validation,
                    applicationValidationException.Message);

                httpContext.Response.StatusCode = validationProblemDetails.Status.Value;

                var problemDetailsContext = new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = validationProblemDetails,
                    Exception = applicationValidationException
                };

                await _problemDetailsService.WriteAsync(problemDetailsContext);

                return true;
            }

            return false;
        }
    }
}
