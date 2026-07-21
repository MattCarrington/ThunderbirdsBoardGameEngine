using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Routing
{
    public interface IRouteFinder
    {
        RouteResult? FindShortestRoute(MovementInput request);
    }
}