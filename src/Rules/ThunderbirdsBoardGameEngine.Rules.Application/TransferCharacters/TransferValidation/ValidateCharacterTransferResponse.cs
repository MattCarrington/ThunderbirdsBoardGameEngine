namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ValidateCharacterTransferResponse(
        TransferCharacterValidationState TransferCharacterValidationOutcome,
        TransferCharacterValidationRejectionReason? TransferCharacterValidationRejectionReason);
}
