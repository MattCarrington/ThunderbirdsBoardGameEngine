using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record RouteResult(IReadOnlyList<LocationCode> Route, int SpacesTravelled);
}
