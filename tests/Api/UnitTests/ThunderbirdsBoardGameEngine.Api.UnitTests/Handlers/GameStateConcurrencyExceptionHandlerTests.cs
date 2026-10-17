using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public partial class GameNotFoundExceptionHandlerTests
    {
        public class GameStateConcurrencyExceptionHandlerTests
        {
            [Fact]
            public async Task TryHandleAsync_WhenGameStateConcurrencyException_ReturnsTrueAsync()
            {
                // Arrange
                var exception = new GameStateConcurrencyException();

                var service = ExceptionHandlerHelper.CreateProblemsDetailService();

                var handler = CreateHandler(service);

                // Act
                var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

                // Assert
                Assert.True(handled);
                Assert.Equal(StatusCodes.Status409Conflict, status);
                Assert.Equal("application/problem+json; charset=utf-8", contentType);
                Assert.Equal(StatusCodes.Status409Conflict, body.Status);
                Assert.Equal("Game state conflict.", body.Title);
                Assert.Equal(ProblemTypes.Conflict, body.Type);
                Assert.Null(body.Instance);

                await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
            }

            [Fact]
            public async Task TryHandleAsync_WhenArgumentNullException_ReturnsFalse()
            {
                // Arrange
                var exception = new ArgumentNullException("Some error");

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

            private static GameStateConcurrencyExceptionHandler CreateHandler(IProblemDetailsService service)
            {
                var factory = new FakeProblemDetailsFactory();
                var logger = NullLogger<GameStateConcurrencyExceptionHandler>.Instance;

                return new GameStateConcurrencyExceptionHandler(factory, service, logger);
            }
        }
    }
}
