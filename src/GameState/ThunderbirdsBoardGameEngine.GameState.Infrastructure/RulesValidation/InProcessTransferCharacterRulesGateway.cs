using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation;

using GameStateThunderbirdState =
    ThunderbirdsBoardGameEngine.GameState.Application
        .TransferCharacters.CurrentThunderbirdState;

using RulesThunderbirdState =
    ThunderbirdsBoardGameEngine.Rules.Application
        .TransferCharacters.TransferValidation.CurrentThunderbirdState;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.RulesValidation
{
    internal sealed class InProcessTransferCharacterRulesGateway : ITransferCharacterRulesGateway
    {
        private readonly IMediator _mediator;

        public InProcessTransferCharacterRulesGateway(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<CharacterTransferDecision> ValidateTransferCharactersAsync(
            IReadOnlyCollection<GameStateThunderbirdState> currentThunderbirdState,
            IReadOnlyCollection<CharacterAssignment> characterAssignments,
            CancellationToken cancellationToken)
        {
            var query = new ValidateCharacterTransferQuery(
                CurrentState: currentThunderbirdState
                    .Select(state =>
                        new RulesThunderbirdState(
                            ThunderbirdCode: state.Thunderbird,
                            CurrentLocation: state.Location,
                            Occupants: state.CharactersAboard))
                    .ToArray(),
                ProposedAssignments: characterAssignments
                    .Select(assignment =>
                        new RequestedCharacterAssignment(
                            CharacterCode: assignment.CharacterCode,
                            TargetThunderbirdCode: assignment.DestinationThunderbirdCode))
                    .ToArray());

            var result = await _mediator.Send(query, cancellationToken);

            if (result.IsPermitted)
            {
                return CharacterTransferDecision.Permitted();
            }

            if (result.RejectionReason is null)
            {
                throw new InvalidOperationException(
                    "A rejected Rules response must provide a reason.");
            }

            return CharacterTransferDecision.Rejected(MapReason(result.RejectionReason.Value));
        }

        private static CharacterTransferRejectionReason MapReason(TransferCharacterValidationRejectionReason reason)
        {
            return reason switch
            {
                TransferCharacterValidationRejectionReason.NotReciprocal => CharacterTransferRejectionReason.NotReciprocal,
                TransferCharacterValidationRejectionReason.MachinesNotCoLocated => CharacterTransferRejectionReason.MachinesNotCoLocated,
                TransferCharacterValidationRejectionReason.CharacterCapacityExceeded => CharacterTransferRejectionReason.CharacterCapacityExceeded,
                _ => throw new ArgumentOutOfRangeException(
                    nameof(reason),
                    reason,
                    "Unexpected rejection reason from Rules."),
            };
        }
    }
}