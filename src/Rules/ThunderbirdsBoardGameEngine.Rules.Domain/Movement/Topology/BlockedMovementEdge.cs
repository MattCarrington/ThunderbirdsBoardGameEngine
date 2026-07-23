using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public sealed record BlockedMovementEdge(LocationCode Location1, LocationCode Location2)
    {
        public bool Matches(ReferenceMapEdgeDefinition edge)
        {
            return (edge.Edge1 == Location1 && edge.Edge2 == Location2)
                || (edge.Edge1 == Location2 && edge.Edge2 == Location1);
        }
    }
}
