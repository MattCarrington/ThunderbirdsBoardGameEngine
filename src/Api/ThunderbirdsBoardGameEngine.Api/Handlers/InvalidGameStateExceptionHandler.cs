using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class InvalidGameStateExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<InvalidGameStateExceptionHandler> _logger;

        public InvalidGameStateExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<InvalidGameStateExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is InvalidGameStateException invalidGameState)
            {
                _logger.LogError(
                    invalidGameState,
                    "Game State integrity validation failed. " +
                    "TraceId = {TraceId}",
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred.",
                    ProblemTypes.Unexpected,
                    "The server could not process the game state.");

                problemDetails.Instance = null;

                httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;

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
