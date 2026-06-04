using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record Topography(
        IReadOnlyCollection<ReferenceMapEdgeDefinition> Edges);
}
