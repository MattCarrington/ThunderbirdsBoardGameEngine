using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public class BadRequestExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<BadRequestExceptionHandler> _logger;

        public BadRequestExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService, ILogger<BadRequestExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is BadRequestException badRequestException)
            {
                _logger.LogWarning(
                     "Bad request for {Method} {Path}. TraceId = {TraceId}",
                     httpContext.Request.Method,
                     httpContext.Request.Path.Value,
                     httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status400BadRequest,
                    "Bad request.",
                    ProblemTypes.BadRequest,
                    badRequestException.Message);

                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status400BadRequest;

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
