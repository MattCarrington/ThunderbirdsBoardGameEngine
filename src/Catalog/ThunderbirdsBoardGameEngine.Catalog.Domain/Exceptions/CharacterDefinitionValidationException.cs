
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    public class CharacterDefinitionValidationException : DomainValidationException
    {
        public CharacterDefinitionErrorCode ErrorCode { get; }

        public Character? Character { get; }

        private CharacterDefinitionValidationException(
            CharacterDefinitionErrorCode errorCode,
            string message,
            Character? character = null,
            Exception? innerException = null) : base(message, innerException)
        {
            ErrorCode = errorCode;
            Character = character;
        }

        public static CharacterDefinitionValidationException Unknown(Character? character = null, Exception? innerException = null)
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.Unknown,
                "An unknown character definition validation error has occurred.",
                character,
                innerException);
        }

        public static CharacterDefinitionValidationException NullEntry()
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.NullEntry,
                "The character definitions collection contains a null entry.");
        }

        public static CharacterDefinitionValidationException DuplicateKey(Character character)
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.DuplicateKey,
                $"A character definition with the key '{character}' already exists.",
                character);
        }

        public static CharacterDefinitionValidationException InvalidCount()
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.InvalidCount,
                $"The number of character definitions must be exactly six.");
        }

        public static CharacterDefinitionValidationException InvalidRescueBonusCount(Character character)
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.InvalidRescueBonusCount,
                $"Character '{character}' has more than one rescue bonus.",
                character);
        }
    }

    public enum CharacterDefinitionErrorCode
    {
        Unknown = 0,
        NullEntry,
        DuplicateKey,
        InvalidCount,
        InvalidRescueBonusCount
    }
}
