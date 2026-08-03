using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class ThunderbirdMachineNotFoundExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_WhenBadRequestException_ReturnsTrueAsync()
        {
            // Arrange
            var exception = ThunderbirdMachineNotFoundException.Create(new ThunderbirdCode("thunderbird-machine-id"));

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status404NotFound, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status404NotFound, body.Status);
            Assert.Equal("Thunderbird Machine was not found.", body.Title);
            Assert.Equal(ProblemTypes.NotFound, body.Type);

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

        private static ThunderbirdMachineNotFoundExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();
            var logger = NullLogger<ThunderbirdMachineNotFoundExceptionHandler>.Instance;

            return new ThunderbirdMachineNotFoundExceptionHandler(factory, service, logger);
        }
    }
}
