namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    public sealed record CharacterTransferDecision(
        bool IsPermitted,
        CharacterTransferRejectionReason? RejectionReason);
}
