namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public sealed record MovementResultViewModel(
        bool IsValid,
        int ActionPointCost,
        int SpacesTravelled,
        int TopSpeed,
        IReadOnlyList<string> Route,
        IReadOnlyList<string> Messages);
}
