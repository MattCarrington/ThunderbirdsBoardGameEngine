using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class CharacterNotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;

        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<CharacterNotFoundExceptionHandler> _logger;

        public CharacterNotFoundExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService,
            ILogger<CharacterNotFoundExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is CharacterNotFoundException)
            {
                _logger.LogWarning(
                    "Requested character was not found. TraceId = {TraceId}",
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status404NotFound,
                    "Character was not found.",
                    ProblemTypes.NotFound,
                    "The requested character does not exist.");

                problemDetails.Instance = null;

                httpContext.Response.StatusCode = StatusCodes.Status404NotFound;

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
