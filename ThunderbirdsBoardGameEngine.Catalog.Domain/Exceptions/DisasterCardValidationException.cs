namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    /// <summary>
    /// Represents a validation failure related to one or more disaster cards.
    /// </summary>
    /// <remarks>
    /// This exception is thrown when catalog-level validation rules are violated,
    /// such as duplicate identifiers or invalid card definitions.
    /// </remarks>
    public class DisasterCardValidationException : DomainValidationException
    {
        /// <summary>
        /// Gets the error code describing the validation failure.
        /// </summary>
        public DisasterCardErrorCode ErrorCode { get; }

        /// <summary>
        /// Gets the identifier of the card involved in the failure, if available.
        /// </summary>
        public int? CardId { get; }

        /// <summary>
        /// Gets the name of the card involved in the failure, if available.
        /// </summary>
        public string? CardName { get; }

        private DisasterCardValidationException(DisasterCardErrorCode errorCode, string message, int? cardId = null, string? cardName = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            CardId = cardId;
            CardName = cardName;
        }

        /// <summary>
        /// Creates an exception representing an unknown error with validating the disaster card deck.
        /// </summary>
        public static DisasterCardValidationException Unknown(int? id = null, string? name = null, Exception? innerException = null)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.Unknown,
                "An unknown disaster card validation error occurred.",
                id,
                name,
                innerException);
        }

        /// <summary>
        /// Creates an exception representing a null entry in the disaster card deck.
        /// </summary>
        public static DisasterCardValidationException NullEntry()
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.NullEntry,
                "The disaster cards collection contains a null entry.",
                null,
                null);
        }

        /// <summary>
        /// Creates an exception representing an ID already in use within the disaster card deck.
        /// </summary>
        public static DisasterCardValidationException DuplicateId(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.DuplicateId,
                $"A disaster card with the id '{id}' already exists. (Name: '{name}')",
                id,
                name);
        }

        /// <summary>
        /// Creates an exception representing a name already in use within the disaster card deck.
        /// </summary>
        public static DisasterCardValidationException DuplicateName(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.DuplicateName,
                $"A disaster card with the Name '{name}' already exists. (ID: {id})",
                id,
                name);
        }

        /// <summary>
        /// Creates an exception representing a code already in use within the disaster card deck.
        /// </summary>
        public static DisasterCardValidationException DuplicateCode(int id, string name, string code)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.DuplicateCode,
                $"A disaster card with the Code '{code}' already exists. (ID: {id}, Name: '{name}')",
                id,
                name);
        }

        /// <summary>
        /// Creates an exception representing a disaster card having no bonus conditions.
        /// </summary>
        public static DisasterCardValidationException NullBonusCondition(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.NullBonusCondition,
                $"A bonus condition is null for card {name}. (ID: {id})",
                id,
                name);
        }

        /// <summary>
        /// Creates an exception representing a disaster card having an unknown bonus condition.
        /// </summary>
        public static DisasterCardValidationException UnknownBonusCondition(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.UnknownBonusCondition,
                $"An unknown bonus condition error occurred for card {name}. (ID: {id})",
                id,
                name);
        }

        /// <summary>
        /// Creates an exception representing a disaster card having no rewards option.
        /// </summary>
        public static DisasterCardValidationException NullRewardOption(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.NullRewardOption,
                $"A reward option is null for card {name}. (ID: {id})",
                id,
                name);
        }

        /// <summary>
        /// Creates an exception representing a disaster card having an unknown reward option.
        /// </summary>
        public static DisasterCardValidationException UnknownRewardOption(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.UnknownRewardOption,
                $"An unknown reward option error occurred for card {name}. (ID: {id})",
                id,
                name);
        }
    }

    /// <summary>
    /// Defines categories of disaster card validation errors.
    /// </summary>
    /// <remarks>
    /// Error codes are stable and intended for use in testing and error handling
    /// without relying on exception message text.
    /// </remarks>
    public enum DisasterCardErrorCode
    {
        Unknown = 0,
        NullEntry,
        DuplicateId,
        DuplicateName,
        DuplicateCode,
        NullBonusCondition,
        UnknownBonusCondition,
        NullRewardOption,
        UnknownRewardOption
    }
}