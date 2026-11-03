using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;

namespace ThunderbirdsBoardGameEngine.GameState.Application.Exceptions
{
    public sealed class CharacterTransferRejectedException : Exception
    {
        public CharacterTransferRejectionReason Reason { get; }

        private CharacterTransferRejectedException(CharacterTransferRejectionReason reason)
            : base("The requested character transfer was rejected.")
        {
            Reason = reason;
        }

        public static CharacterTransferRejectedException NoChange()
        {
            return new(CharacterTransferRejectionReason.NoChange);
        }

        public static CharacterTransferRejectedException FromDecision(CharacterTransferDecision decision)
        {
            if (decision.IsPermitted)
            {
                throw new ArgumentException(
                    "A permitted decision cannot produce a rejection exception.",
                    nameof(decision));
            }

            return new(decision.RejectionReason
                ?? throw new InvalidOperationException(
                    "A rejected transfer decision must provide a reason."));
        }
    }
}
