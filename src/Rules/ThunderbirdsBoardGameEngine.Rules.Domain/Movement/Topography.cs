using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record Topography(
        IReadOnlyCollection<LocationContribution> Locations,
        IReadOnlyCollection<ReferenceMapEdgeDefinition> Edges);
}
