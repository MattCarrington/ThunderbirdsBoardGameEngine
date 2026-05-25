using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceMapEdgeDefinition(LocationCode Edge1, LocationCode Edge2, TraversalDomain EdgeType);
}
