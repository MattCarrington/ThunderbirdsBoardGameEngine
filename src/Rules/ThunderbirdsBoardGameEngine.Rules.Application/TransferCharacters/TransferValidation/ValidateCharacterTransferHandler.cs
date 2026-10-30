using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public class ValidateCharacterTransferHandler : IRequestHandler<ValidateCharacterTransferQuery, ValidateCharacterTransferResponse>
    {
        private readonly IValidateCharacterTransferResolutionService _validationService;

        public ValidateCharacterTransferHandler(IValidateCharacterTransferResolutionService validationService)
        {
            _validationService = validationService;
        }

        public Task<ValidateCharacterTransferResponse> Handle(ValidateCharacterTransferQuery request, CancellationToken cancellationToken)
        {
            var input = new ValidateCharacterTransferInput(
                Thunderbirds: request.GameState.Select(gs => new CurrentThunderbirdState(
                    ThunderbirdCode: gs.ThunderbirdCode,
                    CurrentLocation: gs.CurrentLocation,
                    Occupants: gs.Occupants)).ToArray(),
                Assignments: request.ProposedAssignments.Select(pa => new ProposedCharacterAssignment(
                    CharacterCode: pa.CharacterCode,
                    TargetThunderbirdCode: pa.TargetThunderbirdCode)).ToArray());

            var evaluationResult = _validationService.ResolveTransferValidation(input);

            var response = evaluationResult.Outcome switch
            {
                TransferCharacterOutcome.Valid => new ValidateCharacterTransferResponse(
                    TransferCharacterValidationOutcome: TransferCharacterValidationState.Valid,
                    TransferCharacterValidationRejectionReason: null),
                TransferCharacterOutcome.NoChange => new ValidateCharacterTransferResponse(
                    TransferCharacterValidationOutcome: TransferCharacterValidationState.NoChange,
                    TransferCharacterValidationRejectionReason: null),
                TransferCharacterOutcome.Rejected => new ValidateCharacterTransferResponse(
                    TransferCharacterValidationState.Rejected,
                    TransferCharacterValidationRejectionReason: MapRejectionReason(evaluationResult.RejectionReason)),
                _ => throw new InvalidOperationException($"Unexpected outcome: {evaluationResult.Outcome}")
            };

            return Task.FromResult(response);
        }

        private TransferCharacterValidationRejectionReason? MapRejectionReason(TransferCharacterRejectionReason? rejectionReason)
        {
            ArgumentNullException.ThrowIfNull(rejectionReason, nameof(rejectionReason));

            return rejectionReason switch
            {
                TransferCharacterRejectionReason.NotReciprocal => TransferCharacterValidationRejectionReason.NotReciprocal,
                TransferCharacterRejectionReason.MachinesNotCoLocated => TransferCharacterValidationRejectionReason.MachinesNotCoLocated,
                TransferCharacterRejectionReason.CharacterCapacityExceeded => TransferCharacterValidationRejectionReason.CharacterCapacityExceeded,
                _ => throw new InvalidOperationException($"Unexpected rejection reason: {rejectionReason}")
            };
        }
    }
}
