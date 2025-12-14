using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;

namespace ThunderbirdsBoardGameEngine.Api.Handlers
{
    public sealed class CatalogDataAccessExceptionHandler : IExceptionHandler
    {
        private readonly ProblemDetailsFactory _problemDetailsFactory;
        private readonly IProblemDetailsService _problemDetailsService;
        private readonly ILogger<CatalogDataAccessExceptionHandler> _logger;

        public CatalogDataAccessExceptionHandler(
            ProblemDetailsFactory problemDetailsFactory,
            IProblemDetailsService problemDetailsService,
            ILogger<CatalogDataAccessExceptionHandler> logger)
        {
            _problemDetailsFactory = problemDetailsFactory ?? throw new ArgumentNullException(nameof(problemDetailsFactory));
            _problemDetailsService = problemDetailsService ?? throw new ArgumentNullException(nameof(problemDetailsService));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, Exception exception, CancellationToken cancellationToken)
        {
            if (exception is CatalogDataAccessException catalogDataAccessException)
            {
                var (status, title, type) = catalogDataAccessException.ErrorCode switch
                {
                    CatalogDataAccessErrorCode.Unknown => (StatusCodes.Status500InternalServerError, "An unexpected error occurred while accessing catalog data", ProblemTypes.Unexpected),
                    CatalogDataAccessErrorCode.SourceNotFound => (StatusCodes.Status500InternalServerError, "The catalog data source was not found", ProblemTypes.ServerError),
                    CatalogDataAccessErrorCode.BadJson => (StatusCodes.Status500InternalServerError, "The catalog data source contains invalid JSON", ProblemTypes.ServerError),
                    CatalogDataAccessErrorCode.AccessDenied => (StatusCodes.Status500InternalServerError, "Access to the catalog data source was denied", ProblemTypes.ServerError),
                    CatalogDataAccessErrorCode.SourceUnreadable => (StatusCodes.Status503ServiceUnavailable, "The catalog data source could not be read", ProblemTypes.Unavailable),
                    CatalogDataAccessErrorCode.DataMissing => (StatusCodes.Status500InternalServerError, "The catalog data is missing", ProblemTypes.ServerError),
                    _ => (StatusCodes.Status500InternalServerError, "Catalog error", "about:blank")
                };

                var fileName = Path.GetFileName(catalogDataAccessException.Path);

                var detail = catalogDataAccessException.ErrorCode switch
                {
                    CatalogDataAccessErrorCode.BadJson
                        => $"The catalog content in '{fileName}' is invalid.",
                    CatalogDataAccessErrorCode.DataMissing
                        => $"The catalog data in '{fileName}' is missing or empty.",
                    CatalogDataAccessErrorCode.SourceNotFound
                        => $"The configured catalog file '{fileName}' was not found.",
                    CatalogDataAccessErrorCode.AccessDenied
                        => $"Insufficient permission to read '{fileName}'.",
                    CatalogDataAccessErrorCode.SourceUnreadable
                        => "The catalog source is temporarily unavailable. Please retry later.",
                    _ => "An unexpected catalog error occurred."
                };

                // Optional hint for clients on 503
                if (status == StatusCodes.Status503ServiceUnavailable)
                {
                    httpContext.Response.Headers.TryAdd("Retry-After", "30");
                }

                _logger.LogError(
                    catalogDataAccessException,
                    "Catalog data access error. ErrorCode = {ErrorCode}, Path = {Path}, TraceId = {TraceId}",
                    catalogDataAccessException.ErrorCode,
                    catalogDataAccessException.Path,
                    httpContext.TraceIdentifier);

                var problemDetails = _problemDetailsFactory.CreateProblemDetails(
                    httpContext,
                    status,
                    title,
                    type,
                    detail);

                httpContext.Response.StatusCode = problemDetails.Status ?? StatusCodes.Status500InternalServerError;

                var problemDetailsContext = new ProblemDetailsContext
                {
                    HttpContext = httpContext,
                    ProblemDetails = problemDetails,
                    Exception = catalogDataAccessException
                };

                await _problemDetailsService.WriteAsync(problemDetailsContext);

                return true;
            }

            return false;
        }
    }
}
