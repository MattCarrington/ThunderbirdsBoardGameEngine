using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Routing
{
    public sealed record RouteResult(IReadOnlyList<LocationCode> Route, int SpacesTravelled);
}
