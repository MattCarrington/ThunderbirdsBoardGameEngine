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

            if (decision.RejectionReason is null)
            {
                throw new InvalidOperationException(
                    "A rejected transfer decision must provide a reason.");
            }

            if (decision.RejectionReason == CharacterTransferRejectionReason.NoChange)
            {
                throw new InvalidOperationException(
                    "A no-change rejection cannot originate from the Rules gateway.");
            }

            return new(decision.RejectionReason.Value);
        }
    }
}
