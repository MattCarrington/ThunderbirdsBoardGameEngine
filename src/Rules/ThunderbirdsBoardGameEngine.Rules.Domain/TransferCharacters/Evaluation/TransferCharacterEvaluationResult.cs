namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public record TransferCharacterEvaluationResult(
        TransferCharacterOutcome Outcome,
        TransferCharacterRejectionReason? RejectionReason = null);
}
