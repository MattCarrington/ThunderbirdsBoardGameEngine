using System.Net;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Exceptions
{
    public class ApiRequestException : ApiClientException
    {
        public HttpStatusCode StatusCode { get; }

        public ApiRequestException(string message, HttpStatusCode statusCode)
            : base(message) => StatusCode = statusCode;
    }
}
