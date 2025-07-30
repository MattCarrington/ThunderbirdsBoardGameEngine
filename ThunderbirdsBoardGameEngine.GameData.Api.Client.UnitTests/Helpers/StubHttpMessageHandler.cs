using System.Net;
using System.Text;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.UnitTests.Helpers
{
    public class StubHttpMessageHandler : HttpMessageHandler
    {
        public HttpRequestMessage? CapturedRequest { get; private set; }

        private readonly HttpResponseMessage _response;

        public StubHttpMessageHandler(string content, HttpStatusCode statusCode)
        {
            _response = new HttpResponseMessage(statusCode)
            {
                Content = new StringContent(content, Encoding.UTF8, "application/json")
            };
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            CapturedRequest = request;
            return Task.FromResult(_response);
        }
    }
}
