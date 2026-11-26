using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions
{
    public sealed class CharacterTransferCannotCompleteException : Exception
    {
        private CharacterTransferCannotCompleteException(string message)
            : base(message)
        {
        }

        public static CharacterTransferCannotCompleteException OutsideActiveCharacterLocation(LocationCode location)
        {
            return new CharacterTransferCannotCompleteException(
                $"The transfer location is taking place outside of the active character's current location: {location}");
        }
    }
}
