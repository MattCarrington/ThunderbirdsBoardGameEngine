using System.Net;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure.UnitTests.Handlers
{
    public class DefaultHttpHandlerTests
    {
        [Fact]
        public async Task HandleResponseAsync_WhenValidResponse_ReturnsSuccessApiResult()
        {
            // Arrange
            var handler = new DefaultHttpResponseHandler();
            
            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[1]")
            };
            
            // Act
            
            var result = await handler.HandleResponseAsync<IReadOnlyList<int>>(response, CancellationToken.None);
            
            // Assert
            Assert.NotNull(result);
            Assert.True(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.NotNull(result.Data);
            Assert.Null(result.ErrorMessage);
            var data = Assert.Single(result.Data);
            Assert.Equal(1, data);
        }

        [Fact]
        public async Task HandleResponseAsync_WhenDeserializedContentIsNull_ReturnsFailureApiResult()
        {
            // Arrange
            var handler = new DefaultHttpResponseHandler();

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("null")
            };

            // Act
            var result = await handler.HandleResponseAsync<IReadOnlyList<string>>(response, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Deserialized content was null", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HandleResponseAsync_WhenApiReturnsMalformedJson_ReturnsFailureApiResult()
        {
            // Arrange
            var handler = new DefaultHttpResponseHandler();

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("Invalid JSON")
            };

            // Act
            var result = await handler.HandleResponseAsync<IReadOnlyList<string>>(response, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Deserialization error", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        [Fact]
        public async Task HandleResponseAsync_WhenApiReturnsErrorStatusCode_ReturnsFailureApiResult()
        {
            // Arrange
            var handler = new DefaultHttpResponseHandler();

            using var response = new HttpResponseMessage(HttpStatusCode.BadRequest)
            {
                Content = new StringContent("Bad Request Error Message")
            };

            // Act
            var result = await handler.HandleResponseAsync<IReadOnlyList<string>>(response, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Equal("Bad Request Error Message", result.ErrorMessage);
        }

        [Fact]
        public async Task HandleResponseAsync_WhenCancelled_ThrowsOperationCanceledException()
        {
            // Arrange
            var handler = new DefaultHttpResponseHandler();

            using var token = new CancellationTokenSource();
            await token.CancelAsync();

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new StringContent("[]")
            };

            // Act & Assert
            await Assert.ThrowsAnyAsync<OperationCanceledException>(
                () => handler.HandleResponseAsync<IReadOnlyList<string>>(response, token.Token));
        }

        [Fact]
        public async Task HandleResponse_WhenUnexpectedExceptionOccurs_ReturnsFailure()
        {
            // Arrange
            var handler = new DefaultHttpResponseHandler();

            using var response = new HttpResponseMessage(HttpStatusCode.OK)
            {
                Content = new ThrowingHttpContent()
            };

            // Act
            var result = await handler.HandleResponseAsync<string>(response, CancellationToken.None);

            // Assert
            Assert.False(result.Success);
            Assert.Equal(HttpStatusCode.OK, result.StatusCode);
            Assert.Null(result.Data);
            Assert.NotNull(result.ErrorMessage);
            Assert.Contains("Unexpected error", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
        }

        private sealed class ThrowingHttpContent : HttpContent
        {
            protected override Task SerializeToStreamAsync(
                Stream stream,
                TransportContext? context)
                => throw new InvalidOperationException("Boom");

            protected override bool TryComputeLength(out long length)
            {
                length = 0;
                return true;
            }
        }
    }
}
