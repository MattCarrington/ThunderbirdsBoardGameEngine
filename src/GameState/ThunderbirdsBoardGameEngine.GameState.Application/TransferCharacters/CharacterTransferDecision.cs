namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    public sealed class CharacterTransferDecision
    {
        public bool IsPermitted { get; }

        public CharacterTransferRejectionReason? RejectionReason { get; }

        private CharacterTransferDecision(bool isPermitted, CharacterTransferRejectionReason? rejectionReason)
        {
            IsPermitted = isPermitted;
            RejectionReason = rejectionReason;
        }

        public static CharacterTransferDecision Permitted()
        {
            return new CharacterTransferDecision(true, null);
        }

        public static CharacterTransferDecision Rejected(CharacterTransferRejectionReason rejectionReason)
        {
            if (rejectionReason == CharacterTransferRejectionReason.NoChange)
            {
                throw new ArgumentException("No-change decisions are made by GameState, not the Rules gateway.", nameof(rejectionReason));
            }

            return new CharacterTransferDecision(false, rejectionReason);
        }
    }
}
