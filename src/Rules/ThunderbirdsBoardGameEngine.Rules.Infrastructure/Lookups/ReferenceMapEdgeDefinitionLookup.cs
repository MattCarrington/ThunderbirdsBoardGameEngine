using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal sealed class ReferenceMapEdgeDefinitionLookup : IMapEdgeDefinitionLookup
    {
        private readonly IMapEdgeDefinitionCatalog _mapEdgeDefinitionCatalog;

        public ReferenceMapEdgeDefinitionLookup(IMapEdgeDefinitionCatalog mapEdgeDefinitionCatalog)
        {
            _mapEdgeDefinitionCatalog = mapEdgeDefinitionCatalog;
        }

        public IReadOnlyCollection<ReferenceMapEdgeDefinition> GetAll()
        {
            return _mapEdgeDefinitionCatalog.GetAll();
        }
    }
}
