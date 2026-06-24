using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class InvalidRescueCalculationRequestExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_WhenInvalidRescueCalculationRequestException_ReturnsTrueAsync()
        {
            // Arrange
            var exception = new InvalidRescueCalculationRequestException("type", "message");

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status400BadRequest, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status400BadRequest, body.Status);
            Assert.Equal("Invalid rescue calculation request.", body.Title);
            Assert.Equal(ProblemTypes.BadRequest, body.Type);

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

        private static InvalidRescueCalculationRequestExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();
            var logger = NullLogger<InvalidRescueCalculationRequestExceptionHandler>.Instance;

            return new InvalidRescueCalculationRequestExceptionHandler(factory, service, logger);
        }
    }
}
