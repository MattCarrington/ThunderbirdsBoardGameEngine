namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public record ValidateCharacterTransferResponse(
        bool IsPermitted,
        TransferCharacterValidationRejectionReason? RejectionReason);
}
