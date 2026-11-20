using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class InvalidTurnOrderExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<InvalidTurnOrderExceptionHandler> _logger;

        public InvalidTurnOrderExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<InvalidTurnOrderExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is InvalidTurnOrderException)
            {
                _logger.LogWarning(
                    "Invalid turn order was detected. TraceId = {TraceId}",
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status422UnprocessableEntity,
                    "Invalid turn order was detected.",
                    ProblemTypes.Unprocessable,
                    "The requested turn order is not permitted.");

                problemDetails.Instance = null;

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
