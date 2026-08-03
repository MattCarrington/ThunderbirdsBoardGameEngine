using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class ThunderbirdMachineNotFoundExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<ThunderbirdMachineNotFoundExceptionHandler> _logger;

        public ThunderbirdMachineNotFoundExceptionHandler(ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<ThunderbirdMachineNotFoundExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory;
            _problemDetailsService = problemDetailsService;
            _logger = logger;
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is ThunderbirdMachineNotFoundException machineNotFoundException)
            {
                _logger.LogWarning(
                    "Requested game was not found. TraceId = {TraceId}",
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    StatusCodes.Status404NotFound,
                    "Thunderbird Machine was not found.",
                    ProblemTypes.NotFound,
                    "The requested machine does not exist.");

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
