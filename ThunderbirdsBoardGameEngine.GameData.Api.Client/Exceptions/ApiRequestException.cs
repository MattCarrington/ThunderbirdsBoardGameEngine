using System.Net;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.Exceptions
{
    public class ApiRequestException : ApiClientException
    {
        public HttpStatusCode StatusCode { get; }

        public ApiRequestException(string message, HttpStatusCode statusCode)
            : base(message) => StatusCode = statusCode;
    }
}
