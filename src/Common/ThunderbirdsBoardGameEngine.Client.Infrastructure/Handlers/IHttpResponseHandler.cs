
namespace ThunderbirdsBoardGameEngine.Client.Infrastructure.Handlers
{
    public interface IHttpResponseHandler
    {
        Task<ApiResult<T>> HandleResponseAsync<T>(HttpResponseMessage response, CancellationToken cancellationToken);
    }
}