namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public sealed record CharacterTransferEvaluationInput(
        IReadOnlyCollection<ThunderbirdTransferState> Thunderbirds,
        IReadOnlyCollection<ProposedCharacterAssignment> Assignments);
}
