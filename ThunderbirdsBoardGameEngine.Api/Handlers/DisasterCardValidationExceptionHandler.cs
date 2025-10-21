using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class DisasterCardValidationExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;

        public DisasterCardValidationExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is DisasterCardValidationException)
            {
                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    exception.Message,
                    ProblemTypes.Validation);

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

            return false;
        }
    }
}
