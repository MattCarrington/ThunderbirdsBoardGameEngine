using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeMapEdgeDefinitionCatalog : IMapEdgeDefinitionCatalog
    {
        private readonly ImmutableArray<ReferenceMapEdgeDefinition> _mapEdges;

        public FakeMapEdgeDefinitionCatalog(params ReferenceMapEdgeDefinition[] mapEdges)
        {
            _mapEdges = mapEdges.ToImmutableArray();
        }

        public ImmutableArray<ReferenceMapEdgeDefinition> GetAll()
        {
            return _mapEdges;
        }
    }
}
