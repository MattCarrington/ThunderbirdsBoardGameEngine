namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public sealed record ValidateMovementDecision(bool IsValid, IReadOnlyCollection<string> Messages);
}
