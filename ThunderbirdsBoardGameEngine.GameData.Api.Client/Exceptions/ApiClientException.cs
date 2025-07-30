using System.Runtime.Serialization;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.Exceptions
{
    public abstract class ApiClientException : Exception
    {
        protected ApiClientException()
        {
        }

        protected ApiClientException(string? message) : base(message)
        {
        }

        protected ApiClientException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ApiClientException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
