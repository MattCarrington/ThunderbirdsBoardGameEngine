namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation
{
    public sealed record TransferCharacterEvaluationInput(
        IReadOnlyCollection<ThunderbirdTransferState> Thunderbirds,
        IReadOnlyCollection<ProposedCharacterAssignment> Assignments);
}
