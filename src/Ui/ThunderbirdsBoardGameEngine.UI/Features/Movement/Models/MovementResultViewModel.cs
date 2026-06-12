namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Models
{
    public sealed record MovementResultViewModel(
        bool IsValid,
        int ActionPointCost,
        int SpacesTravelled,
        int TopSpeed,
        IReadOnlyList<string> Route,
        IReadOnlyList<string> Messages);
}
