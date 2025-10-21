using Microsoft.AspNetCore.Http;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class UnhandledExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_WhenUnhandledException_ReturnsTrue()
        {
            // Arrange
            var exception = new Exception("Some unhandled error");

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status500InternalServerError, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status500InternalServerError, body.Status);
            Assert.Equal("An unexpected error occurred.", body.Title);
            Assert.Equal(ProblemTypes.Unexpected, body.Type);

            await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        private static UnhandledExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();
            return new UnhandledExceptionHandler(factory, service);
        }
    }
}
