using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class ThunderbirdMovementRejectedExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_WhenThunderbirdMovementRejectedException_ReturnsTrueAsync()
        {
            // Arrange
            var message = "Thunderbird movement was rejected.";

            var exception = new ThunderbirdMovementRejectedException([message]);

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, body.Status);
            Assert.Equal("Thunderbird Machine movement was rejected.", body.Title);
            Assert.Equal(ProblemTypes.Unprocessable, body.Type);
            Assert.Null(body.Instance);

            var reasons = Assert.IsType<JsonElement>(body.Extensions["reasons"]);
            Assert.Equal(message, reasons[0].GetString());


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

        private static ThunderbirdMovementRejectedExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();
            var logger = NullLogger<ThunderbirdMovementRejectedExceptionHandler>.Instance;

            return new ThunderbirdMovementRejectedExceptionHandler(factory, service, logger);
        }
    }
}
