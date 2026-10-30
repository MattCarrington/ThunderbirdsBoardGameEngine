using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ValidateCharacterTransferQuery(IReadOnlyList<CurrentThunderbirdState> CurrentState, IReadOnlyCollection<RequestedCharacterAssignment> ProposedAssignments)
        : IRequest<ValidateCharacterTransferResponse>;
}
