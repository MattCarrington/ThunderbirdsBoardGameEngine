using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Lookups
{
    internal class ReferenceLocationDefinitionLookup : ILocationDefinitionLookup
    {
        private readonly ILocationDefinitionCatalog _locationDefinitionCatalog;

        public ReferenceLocationDefinitionLookup(ILocationDefinitionCatalog locationDefinitionCatalog)
        {
            _locationDefinitionCatalog = locationDefinitionCatalog;
        }

        public IReadOnlyCollection<LocationContribution> GetAllLocationContributions()
        {
            return _locationDefinitionCatalog.GetAll().Select(MapToLocationContribution).ToList();
        }

        private static LocationContribution MapToLocationContribution(ReferenceLocationDefinition location)
        {
            return new LocationContribution(location.Code, location.Domain);
        }
    }
}
