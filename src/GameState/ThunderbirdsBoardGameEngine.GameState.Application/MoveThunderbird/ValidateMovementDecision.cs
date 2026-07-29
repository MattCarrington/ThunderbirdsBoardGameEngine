namespace ThunderbirdsBoardGameEngine.GameState.Domain.Movement
{
    public sealed record ValidateMovementDecision(bool IsValid, IList<string> Messages);
}
