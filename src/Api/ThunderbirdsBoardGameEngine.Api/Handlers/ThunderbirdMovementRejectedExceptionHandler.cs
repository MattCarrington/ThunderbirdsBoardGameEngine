using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class ThunderbirdMovementRejectedExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<ThunderbirdMovementRejectedExceptionHandler> _logger;

        public ThunderbirdMovementRejectedExceptionHandler(ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<ThunderbirdMovementRejectedExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ThunderbirdMovementRejectedException movementRejectedException)
            {
                _logger.LogWarning(
                    "Requested Thunderbird was not found. TraceId = {TraceId}",
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status422UnprocessableEntity,
                    "Thunderbird Machine movement was rejected.",
                    ProblemTypes.Unprocessable,
                    "The requested movement is not permitted.");

                problemDetails.Instance = null;
                problemDetails.Extensions["reasons"] = movementRejectedException.Reasons;

                httpContext.Response.StatusCode = StatusCodes.Status422UnprocessableEntity;

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
