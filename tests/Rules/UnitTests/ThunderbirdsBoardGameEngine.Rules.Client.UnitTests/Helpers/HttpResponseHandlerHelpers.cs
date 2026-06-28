using NSubstitute;
using ThunderbirdsBoardGameEngine.Client.Core;
using ThunderbirdsBoardGameEngine.Client.Core.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.Client.UnitTests.Helpers
{
    public static class HttpResponseHandlerHelpers
    {

        public static IHttpResponseHandler CreateMockHttpResponseHandler<T>(ApiResult<T> apiResult)
        {
            var handler = Substitute.For<IHttpResponseHandler>();
            handler.HandleResponseAsync<T>(Arg.Any<HttpResponseMessage>(), Arg.Any<CancellationToken>())
                .Returns(apiResult);

            return handler;
        }
    }
}