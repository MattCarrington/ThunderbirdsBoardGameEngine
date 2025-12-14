using System.Net;

namespace ThunderbirdsBoardGameEngine.Catalog.Client
{
    /// <summary>
    /// Represents the result of an API operation, including success state, data, and error details.
    /// </summary>
    /// <typeparam name="T">The type of the data returned by the API.</typeparam>
    public sealed class ApiResult<T>
    {
        /// <summary>
        /// Gets a value indicating whether the API call was successful.
        /// </summary>
        public bool Success { get; init; }

        /// <summary>
        /// Gets the data returned by the API when <see cref="Success"/> is <c>true</c>.
        /// </summary>
        public T? Data { get; init; }

        /// <summary>
        /// Gets an error message describing what went wrong when <see cref="Success"/> is <c>false</c>.
        /// </summary>
        public string? ErrorMessage { get; init; }

        /// <summary>
        /// Gets the HTTP status code returned by the API.
        /// </summary>
        public HttpStatusCode StatusCode { get; init; }

        /// <summary>
        /// Creates a successful <see cref="ApiResult{T}"/> instance.
        /// </summary>
        /// <param name="data">The data returned by the API.</param>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <returns>A successful <see cref="ApiResult{T}"/>.</returns>
        public static ApiResult<T> SuccessResult(T data, HttpStatusCode statusCode)
        {
            return new()
            {
                Success = true,
                Data = data,
                StatusCode = statusCode
            };
        }

        /// <summary>
        /// Creates a failed <see cref="ApiResult{T}"/> instance.
        /// </summary>
        /// <param name="errorMessage">A message describing the error.</param>
        /// <param name="statusCode">The HTTP status code of the response.</param>
        /// <returns>A failed <see cref="ApiResult{T}"/>.</returns>
        public static ApiResult<T> Failure(string errorMessage, HttpStatusCode statusCode)
        {
            return new()
            {
                Success = false,
                ErrorMessage = errorMessage,
                StatusCode = statusCode
            };
        }
    }

}
