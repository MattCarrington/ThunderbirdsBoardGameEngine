using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Api.Error;
using ThunderbirdsBoardGameEngine.Api.Handlers;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Api.UnitTests.Helpers;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Handlers
{
    public class CharacterTransferRejectedExceptionHandlerTests
    {
        [Fact]
        public async Task TryHandleAsync_WhenCharacterTransferRejectedExceptionWithNoChangeReason_ReturnsTrue()
        {
            // Arrange
            var exception = CharacterTransferRejectedException.NoChange();

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, body.Status);
            Assert.Equal("Character transfer was rejected.", body.Title);
            Assert.Equal(ProblemTypes.Unprocessable, body.Type);
            Assert.Null(body.Instance);

            var reason = Assert.IsType<JsonElement>(body.Extensions["reason"]);
            Assert.Equal("no-change", reason.GetString());

            await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        [Fact]
        public async Task TryHandleAsync_WhenCharacterTransferRejectedExceptionWithMachinesNotCoLocatedReason_ReturnsTrue()
        {
            // Arrange
            var exception = CharacterTransferRejectedException.FromDecision(CharacterTransferDecision.Rejected(CharacterTransferRejectionReason.MachinesNotCoLocated));

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, body.Status);
            Assert.Equal("Character transfer was rejected.", body.Title);
            Assert.Equal(ProblemTypes.Unprocessable, body.Type);
            Assert.Null(body.Instance);

            var reason = Assert.IsType<JsonElement>(body.Extensions["reason"]);
            Assert.Equal("machines-not-co-located", reason.GetString());

            await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        [Fact]
        public async Task TryHandleAsync_WhenCharacterTransferRejectedExceptionWithCharacterCapacityExceededReason_ReturnsTrue()
        {
            // Arrange
            var exception = CharacterTransferRejectedException.FromDecision(CharacterTransferDecision.Rejected(CharacterTransferRejectionReason.CharacterCapacityExceeded));

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, body.Status);
            Assert.Equal("Character transfer was rejected.", body.Title);
            Assert.Equal(ProblemTypes.Unprocessable, body.Type);
            Assert.Null(body.Instance);

            var reason = Assert.IsType<JsonElement>(body.Extensions["reason"]);
            Assert.Equal("character-capacity-exceeded", reason.GetString());

            await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        [Fact]
        public async Task TryHandleAsync_WhenCharacterTransferRejectedExceptionWithNotReciprocalReason_ReturnsTrue()
        {
            // Arrange
            var exception = CharacterTransferRejectedException.FromDecision(CharacterTransferDecision.Rejected(CharacterTransferRejectionReason.NotReciprocal));

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act
            var (handled, status, contentType, body) = await ExceptionHandlerHelper.InvokeAsync(handler, exception);

            // Assert
            Assert.True(handled);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, status);
            Assert.Equal("application/problem+json; charset=utf-8", contentType);
            Assert.Equal(StatusCodes.Status422UnprocessableEntity, body.Status);
            Assert.Equal("Character transfer was rejected.", body.Title);
            Assert.Equal(ProblemTypes.Unprocessable, body.Type);
            Assert.Null(body.Instance);

            var reason = Assert.IsType<JsonElement>(body.Extensions["reason"]);
            Assert.Equal("not-reciprocal", reason.GetString());

            await service.Received(1).WriteAsync(Arg.Any<ProblemDetailsContext>());
        }

        [Fact]
        public async Task TryHandleAsync_WhenCharacterTransferRejectedExceptionWithUnknownReason_ThrowsArgumentOutOfRangeException()
        {
            // Arrange
            var exception = CharacterTransferRejectedException.FromDecision(CharacterTransferDecision.Rejected((CharacterTransferRejectionReason)999));

            var service = ExceptionHandlerHelper.CreateProblemsDetailService();

            var handler = CreateHandler(service);

            // Act & Assert
            await Assert.ThrowsAsync<ArgumentOutOfRangeException>(() => ExceptionHandlerHelper.InvokeAsync(handler, exception));
        }

        [Fact]
        public async Task TryHandleAsync_WhenInvalidCastException_ReturnsFalse()
        {
            // Arrange
            var exception = new InvalidCastException("Some error");

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

        private static CharacterTransferRejectedExceptionHandler CreateHandler(IProblemDetailsService service)
        {
            var factory = new FakeProblemDetailsFactory();
            var logger = NullLogger<CharacterTransferRejectedExceptionHandler>.Instance;

            return new CharacterTransferRejectedExceptionHandler(factory, service, logger);
        }
    }
}
