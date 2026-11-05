namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public sealed record TransferCharacterEvaluationResult
    {
        public bool IsPermitted { get; set; }

        public TransferCharacterRejectionReason? RejectionReason { get; }

        private TransferCharacterEvaluationResult(
            bool isPermitted,
            TransferCharacterRejectionReason? rejectionReason = null)
        {
            IsPermitted = isPermitted;
            RejectionReason = rejectionReason;
        }

        public static TransferCharacterEvaluationResult Permitted()
        {
            return new(true);
        }

        public static TransferCharacterEvaluationResult Rejected(
            TransferCharacterRejectionReason reason)
        {
            return new(
                false,
                reason);
        }
    }
}