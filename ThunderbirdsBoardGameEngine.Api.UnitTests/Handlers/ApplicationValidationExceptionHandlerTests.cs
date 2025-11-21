using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class ApplicationValidationExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_WhenApplicationException_ReturnsTrue()
        {
            // Arrange
            var errors = new Dictionary<string, string[]> { ["Field"] = new[] { "Required" } };

            var exception = new ApplicationValidationException("Validation failed", errors);

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();
            
            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync<ValidationProblemDetails>(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status500InternalServerError, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status500InternalServerError, body.Status);
            Assert.Equal("Application validation failed", body.Title);
            Assert.Equal(ProblemTypes.Validation, body.Type);

            Assert.True(body.Errors.TryGetValue("Field", out var messages));
            Assert.Contains("Required", messages);

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
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync<ValidationProblemDetails>(handler, exception);

            // Assert
            Assert.False(handled);
            Assert.Equal(200, status);
            Assert.Equal(string.Empty, contentType);
            Assert.Null(body);

            await service.DidNotReceive().WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        private static ApplicationValidationExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();

            var logger = NullLogger<ApplicationValidationExceptionHandler>.Instance;

            return new ApplicationValidationExceptionHandler(factory, service, logger);
        }
    }
}
