using MediatR;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ValidateCharacterTransferQuery(IReadOnlyList<ThunderbirdGameState> GameState, IReadOnlyCollection<ProposedAssignments> ProposedAssignments)
        : IRequest<ValidateCharacterTransferResponse>;
}
