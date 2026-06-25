using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed record Topography(
        IReadOnlyCollection<ReferenceMapEdgeDefinition> Edges)
    {
        public IEnumerable<LocationCode> GetNeighbours(LocationCode location, MovementDomain domain)
        {
            return Edges
                .Where(edge => edge.EdgeType == domain)
                .Where(edge => edge.Edge1 == location || edge.Edge2 == location)
                .Select(edge => edge.Edge1 == location ? edge.Edge2 : edge.Edge1);
        }

        public IReadOnlyCollection<LocationCode> GetAccessibleLocationsForDomain(MovementDomain domain)
        {
            return Edges
                .Where(edge => edge.EdgeType == domain)
                .SelectMany(edge => new[] { edge.Edge1, edge.Edge2 })
                .Distinct()
                .ToList();
        }
    }
}
