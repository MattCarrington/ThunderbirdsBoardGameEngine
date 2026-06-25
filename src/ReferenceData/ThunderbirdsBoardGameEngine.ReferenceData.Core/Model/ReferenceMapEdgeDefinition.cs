using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceMapEdgeDefinition
    {
        public LocationCode Edge1 { get; }

        public LocationCode Edge2 { get; }

        public MovementDomain EdgeType { get; }

        public ReferenceMapEdgeDefinition(LocationCode edge1, LocationCode edge2, MovementDomain edgeType)
        {
            if (edge1 == edge2)
            {
                throw new ArgumentException("An edge cannot connect a location to itself.");
            }

            Edge1 = edge1;
            Edge2 = edge2;
            EdgeType = edgeType;
        }
    }
}
