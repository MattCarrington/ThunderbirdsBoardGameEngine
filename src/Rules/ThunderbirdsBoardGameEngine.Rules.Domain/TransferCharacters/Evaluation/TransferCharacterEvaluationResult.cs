namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public sealed record TransferCharacterEvaluationResult
    {
        public TransferCharacterOutcome Outcome { get; }

        public TransferCharacterRejectionReason? RejectionReason { get; }

        private TransferCharacterEvaluationResult(
            TransferCharacterOutcome outcome,
            TransferCharacterRejectionReason? rejectionReason = null)
        {
            Outcome = outcome;
            RejectionReason = rejectionReason;
        }

        public static TransferCharacterEvaluationResult Valid()
        {
            return new(TransferCharacterOutcome.Valid);
        }

        public static TransferCharacterEvaluationResult NoChange()
        {
            return new(TransferCharacterOutcome.NoChange);
        }

        public static TransferCharacterEvaluationResult Rejected(
            TransferCharacterRejectionReason reason)
        {
            return new(
                TransferCharacterOutcome.Rejected,
                reason);
        }
    }
}