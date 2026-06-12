using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record RouteResult(IReadOnlyList<LocationCode> Route, int SpacesTravelled);
}
