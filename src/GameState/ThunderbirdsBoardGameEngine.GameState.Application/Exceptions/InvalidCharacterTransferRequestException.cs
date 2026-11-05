namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class InvalidCharacterTransferRequestException : Exception
    {
        private InvalidCharacterTransferRequestException(string message)
            : base(message)
        {
        }

        public static InvalidCharacterTransferRequestException NoAssignments()
        {
            return new InvalidCharacterTransferRequestException("No character assignments were provided.");
        }

        public static InvalidCharacterTransferRequestException TooManyAssignments()
        {
            return new InvalidCharacterTransferRequestException("Only a maximum of two character assignments are supported.");
        }

        public static InvalidCharacterTransferRequestException MustInvolveExactlyTwoMachines()
        {
            return new InvalidCharacterTransferRequestException("The character transfer must involve exactly two machines.");
        }

        public static InvalidCharacterTransferRequestException DuplicateCharacterAssignment()
        {
            return new InvalidCharacterTransferRequestException("Duplicate character assignments are not allowed.");
        }
    }
}
