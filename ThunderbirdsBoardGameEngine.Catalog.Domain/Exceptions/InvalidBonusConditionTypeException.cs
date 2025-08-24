using System.Runtime.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    [Serializable]
    public class InvalidBonusConditionTypeException : Exception
    {
        public InvalidBonusConditionTypeException()
        {
        }

        public InvalidBonusConditionTypeException(string? message) : base(message)
        {
        }

        public InvalidBonusConditionTypeException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidBonusConditionTypeException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
