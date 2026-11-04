using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class CharacterTransferRejectedExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<CharacterTransferRejectedExceptionHandler> _logger;

        public CharacterTransferRejectedExceptionHandler(ProblemDetailsFactory problemDetailsFactory, IProblemDetailsService problemDetailsService, ILogger<CharacterTransferRejectedExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is CharacterTransferRejectedException characterTransferRejectedException)
            {
                _logger.LogWarning(
                    "Requested character transfer was rejected. TraceId = {TraceId}",
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status422UnprocessableEntity,
                    "Character transfer was rejected.",
                    ProblemTypes.Unprocessable,
                    "The requested transfer is not permitted.");

                problemDetails.Instance = null;
                problemDetails.Extensions["reason"] = MapReason(characterTransferRejectedException.Reason);

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

        private static string MapReason(CharacterTransferRejectionReason reason)
        {
            return reason switch
            {
                CharacterTransferRejectionReason.NoChange => "no-change",

                CharacterTransferRejectionReason.NotReciprocal => "not-reciprocal",

                CharacterTransferRejectionReason.MachinesNotCoLocated => "machines-not-co-located",

                CharacterTransferRejectionReason.CharacterCapacityExceeded => "character-capacity-exceeded",

                _ => throw new ArgumentOutOfRangeException(
                    nameof(reason),
                    reason,
                    null)
            };
        }
    }
}
