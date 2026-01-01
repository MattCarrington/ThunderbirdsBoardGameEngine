using System.Net;
using ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers;
using ThunderbirdsBoardGameEngine.TestUtils.Stubs;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure.UnitTests.Handlers
{
    public class ApiVersionHeaderHandlerTests
    {
        private const string HeaderName = "X-Api-Version";

        private readonly StubHttpMessageHandler _stub = new("{}", HttpStatusCode.OK);

        [Theory]
        [InlineData("2.1")]
        [InlineData("2025-09-01")]
        public async Task SendAsync_WhenVersionProvided_SetsVersionHeader(string version)
        {
            // Arrange
            var handler = new ApiVersionHeaderHandler(version);
            using var req = new HttpRequestMessage(HttpMethod.Get, "http://example.test/foo");

            // Act
            _ = await SendThroughAsync(handler, _stub, req);

            // Assert
            Assert.NotNull(_stub.CapturedRequest);
            Assert.True(_stub.CapturedRequest!.Headers.TryGetValues(HeaderName, out var values));
            Assert.Equal(version, Assert.Single(values));
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public async Task SendAsync_WhenNoVersionProvided_SetsDefaultVersionHeader(string? version)
        {
            // Arrange
            var handler = new ApiVersionHeaderHandler(version!);
            using var req = new HttpRequestMessage(HttpMethod.Get, "http://example.test/foo");

            // Act
            _ = await SendThroughAsync(handler, _stub, req);

            // Assert
            Assert.True(_stub.CapturedRequest!.Headers.TryGetValues(HeaderName, out var values));
            Assert.Equal("1.0", Assert.Single(values));
        }

        [Fact]
        public async Task SendAsync_WhenPreExistingHeaderSet_ReplacesPreExistingHeader()
        {
            var handler = new ApiVersionHeaderHandler("3.0");
            using var req = new HttpRequestMessage(HttpMethod.Get, "http://example.test/foo");
            req.Headers.Add(HeaderName, "old-value"); // simulate pre-existing header

            _ = await SendThroughAsync(handler, _stub, req);

            Assert.True(_stub.CapturedRequest!.Headers.TryGetValues(HeaderName, out var values));
            Assert.Equal("3.0", Assert.Single(values)); // replaced, not duplicated
        }

        [Fact]
        public async Task Forwards_Request_To_Inner_Handler_Exactly_Once()
        {
            var handler = new ApiVersionHeaderHandler("2.0");
            using var req = new HttpRequestMessage(HttpMethod.Get, "http://example.test/foo");

            _ = await SendThroughAsync(handler, _stub, req);

            Assert.Equal(1, _stub.CallCount);
        }

        private static async Task<HttpResponseMessage> SendThroughAsync(ApiVersionHeaderHandler handler, HttpMessageHandler inner, HttpRequestMessage request)
        {
            handler.InnerHandler = inner;
            using var invoker = new HttpMessageInvoker(handler);
            return await invoker.SendAsync(request, CancellationToken.None);
        }
    }
}
