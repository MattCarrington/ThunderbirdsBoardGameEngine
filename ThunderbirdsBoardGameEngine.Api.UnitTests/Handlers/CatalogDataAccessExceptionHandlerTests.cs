using Microsoft.AspNetCore.Http;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using Xunit;
using Xunit.Abstractions;

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
            return new CatalogDataAccessExceptionHandler(factory, service);
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

        public class CatalogDataAccessExceptionHandlerTestCase : IXunitSerializable
        {
            public CatalogDataAccessErrorCode ErrorCode { get; private set; }

            public int ExpectedStatus { get; private set; }

            public string ExpectedTitle { get; private set; }

            public string ExpectedType { get; private set; }

            [Obsolete("Called by the de-serializer; should only be called by deriving classes for de-serialization purposes")]
            public CatalogDataAccessExceptionHandlerTestCase()
            {
            }

            public CatalogDataAccessExceptionHandlerTestCase(CatalogDataAccessErrorCode errorCode, int expectedStatus, string expectedTitle, string expectedType)
            {
                ErrorCode = errorCode;
                ExpectedStatus = expectedStatus;
                ExpectedTitle = expectedTitle;
                ExpectedType = expectedType;
            }

            public void Serialize(IXunitSerializationInfo info)
            {
                info.AddValue(nameof(ErrorCode), ErrorCode);
                info.AddValue(nameof(ExpectedStatus), ExpectedStatus);
                info.AddValue(nameof(ExpectedTitle), ExpectedTitle);
                info.AddValue(nameof(ExpectedType), ExpectedType);
            }

            public void Deserialize(IXunitSerializationInfo info)
            {
                ErrorCode = info.GetValue<CatalogDataAccessErrorCode>(nameof(ErrorCode));
                ExpectedStatus = info.GetValue<int>(nameof(ExpectedStatus));
                ExpectedTitle = info.GetValue<string>(nameof(ExpectedTitle))!;
                ExpectedType = info.GetValue<string>(nameof(ExpectedType))!;
            }            
        }
    }
}
