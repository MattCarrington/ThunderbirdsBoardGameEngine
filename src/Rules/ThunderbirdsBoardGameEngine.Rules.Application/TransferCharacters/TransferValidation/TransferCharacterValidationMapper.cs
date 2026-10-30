using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public static class TransferCharacterValidationMapper
    {
        public static ValidateCharacterTransferResponse ToResponse(this TransferCharacterEvaluationResult result)
        {
            return result.Outcome switch
            {
                TransferCharacterOutcome.Valid =>
                    new(
                        TransferCharacterValidationState.Valid,
                        TransferCharacterValidationRejectionReason: null),

                TransferCharacterOutcome.NoChange =>
                    new(
                        TransferCharacterValidationState.NoChange,
                        TransferCharacterValidationRejectionReason: null),

                TransferCharacterOutcome.Rejected
                    when result.RejectionReason is not null =>
                    new(
                        TransferCharacterValidationState.Rejected,
                        MapRejectionReason(result.RejectionReason)),

                _ => throw new ArgumentOutOfRangeException(
                    nameof(result),
                    result.Outcome,
                    "Unknown transfer outcome.")
            };
        }

        private static TransferCharacterValidationRejectionReason MapRejectionReason(TransferCharacterRejectionReason? reason)
        {
            return reason switch
            {
                TransferCharacterRejectionReason.NotReciprocal =>
                    TransferCharacterValidationRejectionReason.NotReciprocal,

                TransferCharacterRejectionReason.MachinesNotCoLocated =>
                    TransferCharacterValidationRejectionReason.MachinesNotCoLocated,

                TransferCharacterRejectionReason.CharacterCapacityExceeded =>
                    TransferCharacterValidationRejectionReason.CharacterCapacityExceeded,

                _ => throw new ArgumentOutOfRangeException(
                    nameof(reason),
                    reason,
                    "Unknown transfer rejection reason.")
            };
        }
    }
}
