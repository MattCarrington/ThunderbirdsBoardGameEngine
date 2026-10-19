namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public record CharacterTransferEvaluationResult(
        CharacterTransferOutcome Outcome,
        CharacterTransferRejectionReason? RejectionReason = null);
}
