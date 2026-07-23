using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public sealed record AppliedMovementTopologyModifier(
        CardCode Card,
        IReadOnlyCollection<BlockedMovementEdge> BlockedEdges,
        string Message);
}
