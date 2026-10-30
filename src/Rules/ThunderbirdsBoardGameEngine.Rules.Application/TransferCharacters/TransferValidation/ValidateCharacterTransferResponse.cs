namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ValidateCharacterTransferResponse(
        TransferCharacterValidationState CharacterTransferValidationOutcome,
        TransferCharacterValidationRejectionReason? CharacterTransferValidationRejectionReason);
}
