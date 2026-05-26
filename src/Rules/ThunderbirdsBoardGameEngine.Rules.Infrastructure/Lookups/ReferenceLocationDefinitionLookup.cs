using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal class ReferenceLocationDefinitionLookup : ILocationDefinitionLookup
    {
        private readonly ILocationDefinitionCatalog _locationDefinitionCatalog;

        public ReferenceLocationDefinitionLookup(ILocationDefinitionCatalog locationDefinitionCatalog)
        {
            _locationDefinitionCatalog = locationDefinitionCatalog;
        }

        public IReadOnlyCollection<ReferenceLocationDefinition> GetAll()
        {
            return _locationDefinitionCatalog.GetAll();
        }
    }
}
