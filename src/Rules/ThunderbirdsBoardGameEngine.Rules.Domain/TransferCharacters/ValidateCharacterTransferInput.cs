using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters
{
    public sealed record ValidateCharacterTransferInput(
        IReadOnlyCollection<CurrentThunderbirdState> Thunderbirds,
        IReadOnlyCollection<ProposedCharacterAssignment> Assignments);
}
