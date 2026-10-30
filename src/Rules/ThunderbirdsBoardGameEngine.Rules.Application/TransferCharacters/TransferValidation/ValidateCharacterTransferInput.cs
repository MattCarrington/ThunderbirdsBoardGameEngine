using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public sealed record ValidateCharacterTransferInput(
        IReadOnlyCollection<CurrentThunderbirdState> Thunderbirds,
        IReadOnlyCollection<RequestedCharacterAssignment> Assignments);
}
