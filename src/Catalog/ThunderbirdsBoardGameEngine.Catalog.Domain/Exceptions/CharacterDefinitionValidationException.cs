
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    /// <summary>
    /// Represents an exception that is thrown when a character definition fails validation in the domain model.
    /// </summary>
    /// <remarks>This exception provides additional context about the specific validation error through the
    /// <see cref="ErrorCode"/> property, and may include the related <see cref="Character"/> instance when applicable.
    /// Use the provided static factory methods to create exceptions for specific validation scenarios.</remarks>
    public class CharacterDefinitionValidationException : DomainValidationException
    {
        /// <summary>
        /// Gets the error code that indicates the specific reason for the character definition failure.
        /// </summary>
        public CharacterDefinitionErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the character associated with this instance, if any.
        /// </summary>
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

        /// <summary>
        /// Creates a new instance of the CharacterDefinitionValidationException class that represents an unknown
        /// character definition validation error.
        /// </summary>
        /// <param name="character">The character associated with the validation error, or null if not applicable.</param>
        /// <param name="innerException">The exception that is the cause of this exception, or null if no inner exception is specified.</param>
        /// <returns>A <see cref="CharacterDefinitionValidationException"/> representing an unknown character definition validation error.</returns>
        public static CharacterDefinitionValidationException Unknown(Character? character = null, Exception? innerException = null)
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.Unknown,
                "An unknown character definition validation error has occurred.",
                character,
                innerException);
        }

        /// <summary>
        /// Creates a new CharacterDefinitionValidationException that indicates a null entry was found in the character
        /// definitions collection.
        /// </summary>
        /// <returns>A <see cref="CharacterDefinitionValidationException"/> representing the error condition where a null entry exists in the
        /// character definitions collection.</returns>
        public static CharacterDefinitionValidationException NullEntry()
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.NullEntry,
                "The character definitions collection contains a null entry.");
        }

        /// <summary>
        /// Creates a new exception indicating that a character definition with the specified key already exists.
        /// </summary>
        /// <param name="character">The character whose key caused the duplication error. Cannot be null.</param>
        /// <returns>A <see cref="CharacterDefinitionValidationException"/> representing a duplicate key error for the specified character.</returns>
        public static CharacterDefinitionValidationException DuplicateKey(Character character)
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.DuplicateKey,
                $"A character definition with the key '{character}' already exists.",
                character);
        }

        /// <summary>
        /// Creates a new exception indicating that the number of character definitions is not exactly six.
        /// </summary>
        /// <returns>A <see cref="CharacterDefinitionValidationException"/> representing an invalid character count error.</returns>
        public static CharacterDefinitionValidationException InvalidCount()
        {
            return new CharacterDefinitionValidationException(
                CharacterDefinitionErrorCode.InvalidCharacterCount,
                $"The number of character definitions must be exactly six.");
        }

        /// <summary>
        /// Creates a CharacterDefinitionValidationException that indicates the specified character has more than one
        /// rescue bonus assigned.
        /// </summary>
        /// <param name="character">The character for which the validation error occurred. Cannot be null.</param>
        /// <returns>A <see cref="CharacterDefinitionValidationException"/> representing the invalid rescue bonus count error for the specified
        /// character.</returns>
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
        InvalidCharacterCount,
        InvalidRescueBonusCount
    }
}
