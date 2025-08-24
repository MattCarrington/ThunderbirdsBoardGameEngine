using System.Runtime.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    [Serializable]
    public class DisasterCardValidationException : Exception
    {
        public DisasterCardValidationException()
        {
        }

        public DisasterCardValidationException(string? message) : base(message)
        {
        }

        public DisasterCardValidationException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected DisasterCardValidationException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}