using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.ClassData;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class CatalogDataAccessExceptionHandlerTests
    {
        [Theory]
        [MemberData(nameof(GetCatalogDataAccessExceptionMapperTestCases))]
        public async Task TryHandleAsync_WhenCatalogDataException_ReturnsTrue(CatalogDataAccessExceptionHandlerTestCase testCase)
        {
            // Arrange
            var exception = new CatalogDataAccessException(testCase.ErrorCode, "/var/app/data/cards.json", null);

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(testCase.ExpectedStatus, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(testCase.ExpectedStatus, body.Status);
            Assert.Equal(testCase.ExpectedTitle, body.Title);
            Assert.Equal(testCase.ExpectedType, body.Type);

            await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        [Fact]
        public async Task TryHandleAsync_WhenInvalidOperationException_ReturnsFalse()
        {
            // Arrange
            var exception = new InvalidOperationException("Some error");

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.False(handled);
            Assert.Equal(200, status);
            Assert.Equal(string.Empty, contentType);
            Assert.Null(body);

            await service.DidNotReceive().WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        private static CatalogDataAccessExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();
            var logger = NullLogger<CatalogDataAccessExceptionHandler>.Instance;

            return new CatalogDataAccessExceptionHandler(factory, service, logger);
        }

        public static TheoryData<CatalogDataAccessExceptionHandlerTestCase> GetCatalogDataAccessExceptionMapperTestCases()
        {
            return
            [
                new CatalogDataAccessExceptionHandlerTestCase(
                    CatalogDataAccessErrorCode.Unknown,
                    StatusCodes.Status500InternalServerError,
                    "An unexpected error occurred while accessing catalog data",
                    ProblemTypes.Unexpected),
                new CatalogDataAccessExceptionHandlerTestCase(
                    CatalogDataAccessErrorCode.SourceNotFound,
                    StatusCodes.Status500InternalServerError,
                    "The catalog data source was not found",
                    ProblemTypes.ServerError),
                new CatalogDataAccessExceptionHandlerTestCase(
                    CatalogDataAccessErrorCode.BadJson,
                    StatusCodes.Status500InternalServerError,
                    "The catalog data source contains invalid JSON",
                    ProblemTypes.ServerError),
                new CatalogDataAccessExceptionHandlerTestCase(
                    CatalogDataAccessErrorCode.AccessDenied,
                    StatusCodes.Status500InternalServerError,
                    "Access to the catalog data source was denied",
                    ProblemTypes.ServerError),
                new CatalogDataAccessExceptionHandlerTestCase(
                    CatalogDataAccessErrorCode.SourceUnreadable,
                    StatusCodes.Status503ServiceUnavailable,
                    "The catalog data source could not be read",
                    ProblemTypes.Unavailable),
                new CatalogDataAccessExceptionHandlerTestCase(
                    CatalogDataAccessErrorCode.DataMissing,
                    StatusCodes.Status500InternalServerError,
                    "The catalog data is missing",
                    ProblemTypes.ServerError)
            ];
        }
    }
}
