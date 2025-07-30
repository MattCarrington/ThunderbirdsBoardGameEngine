using System.Net;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client
{
    public class ApiResult<T>
    {
        public bool Success { get; init; }
        public T? Data { get; init; }
        public string? ErrorMessage { get; init; }
        public HttpStatusCode? StatusCode { get; init; }

        public static ApiResult<T> SuccessResult(T data, HttpStatusCode code) => new()
        {
            Success = true,
            Data = data,
            StatusCode = code
        };

        public static ApiResult<T> Failure(string error, HttpStatusCode code) => new()
        {
            Success = false,
            ErrorMessage = error,
            StatusCode = code
        };
    }

}
