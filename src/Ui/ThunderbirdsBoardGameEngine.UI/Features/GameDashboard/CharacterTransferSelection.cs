namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard
{
    public sealed record CharacterTransferSelection(
        string SourceThunderbirdCode,
        string CharacterCode,
        string DestinationThunderbirdCode, string? ReturningCharacterCode);
}
