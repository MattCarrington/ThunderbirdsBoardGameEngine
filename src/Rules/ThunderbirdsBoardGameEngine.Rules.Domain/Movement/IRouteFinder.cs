namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public interface IRouteFinder
    {
        RouteResult? FindShortestRoute(MovementInput request);
    }
}