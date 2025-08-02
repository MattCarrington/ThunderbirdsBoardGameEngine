using System.Runtime.Serialization;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Exceptions
{
    [Serializable]
    public class InvalidRewardConditionException : Exception
    {
        public InvalidRewardConditionException()
        {
        }

        public InvalidRewardConditionException(string? message) : base(message)
        {
        }

        public InvalidRewardConditionException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected InvalidRewardConditionException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}
