using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public static class TransferCharacterValidationMapper
    {
        public static ValidateCharacterTransferResponse ToResponse(this TransferCharacterEvaluationResult result)
        {
            if (result.IsPermitted)
            {
                return new ValidateCharacterTransferResponse(true, null);
            }

            if (!result.IsPermitted && result.RejectionReason is not null)
            {
                return new ValidateCharacterTransferResponse(false, MapRejectionReason(result.RejectionReason));
            }

            throw new ArgumentOutOfRangeException(
                nameof(result),
                result.RejectionReason,
                "Unknown transfer outcome.");
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
