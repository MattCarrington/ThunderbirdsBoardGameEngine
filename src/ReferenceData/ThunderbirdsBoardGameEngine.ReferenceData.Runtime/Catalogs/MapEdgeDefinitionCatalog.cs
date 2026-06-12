using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal class MapEdgeDefinitionCatalog : IMapEdgeDefinitionCatalog
    {
        private readonly ImmutableArray<ReferenceMapEdgeDefinition> _mapEdges;

        public MapEdgeDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _mapEdges = snapshot.MapEdgeDefinitions.ToImmutableArray();
        }

        public ImmutableArray<ReferenceMapEdgeDefinition> GetAll()
        {
            return _mapEdges;
        }
    }
}
