using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public class UnhandledExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;

        public UnhandledExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                httpContext,
                StatusCodes.Status500InternalServerError,
                "An unexpected error occurred.",
                ProblemTypes.Unexpected);

            httpContext.Response.StatusCode = problemDetails.Status.Value;

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
