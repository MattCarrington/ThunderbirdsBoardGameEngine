
namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions
{
    public class DisasterCardValidationException : DomainValidationException
    {
        public DisasterCardErrorCode ErrorCode { get; }

        public int? CardId { get; }

        public string? CardName { get; }

        private DisasterCardValidationException(DisasterCardErrorCode errorCode, string message, int? cardId = null, string? cardName = null, Exception? innerException = null)
            : base(message, innerException)
        {
            ErrorCode = errorCode;
            CardId = cardId;
            CardName = cardName;
        }

        public static DisasterCardValidationException Unknown(int? id = null, string? name = null, Exception? innerException = null)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.Unknown,
                "An unknown disaster card validation error occurred.",
                id,
                name,
                innerException);
        }

        public static DisasterCardValidationException NullEntry()
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.NullEntry,
                "The disaster cards collection contains a null entry.",
                null,
                null);
        }

        public static DisasterCardValidationException DuplicateId(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.DuplicateId,
                $"A disaster card with the id '{id}' already exists. (Name: '{name}')",
                id,
                name);
        }

        public static DisasterCardValidationException DuplicateName(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.DuplicateName,
                $"A disaster card with the Name '{name}' already exists. (ID: {id})",
                id,
                name);
        }

        public static DisasterCardValidationException DuplicateCode(int id, string name, string code)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.DuplicateCode,
                $"A disaster card with the Code '{code}' already exists. (ID: {id}, Name: '{name}')",
                id,
                name);
        }

        public static DisasterCardValidationException NullBonusCondition(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.NullBonusCondition,
                $"A bonus condition is null for card {name}. (ID: {id})",
                id,
                name);
        }

        public static DisasterCardValidationException UnknownBonusCondition(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.UnknownBonusCondition,
                $"An unknown bonus condition error occurred for card {name}. (ID: {id})",
                id,
                name);
        }

        public static DisasterCardValidationException NullRewardOption(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.NullRewardOption,
                $"A reward option is null for card {name}. (ID: {id})",
                id,
                name);
        }

        public static DisasterCardValidationException UnknownRewardOption(int id, string name)
        {
            return new DisasterCardValidationException(
                DisasterCardErrorCode.UnknownRewardOption,
                $"An unknown reward option error occurred for card {name}. (ID: {id})",
                id,
                name);
        }
    }

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