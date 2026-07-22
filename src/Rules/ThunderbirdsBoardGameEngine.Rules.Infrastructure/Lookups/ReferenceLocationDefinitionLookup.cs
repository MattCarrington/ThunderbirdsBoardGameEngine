using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Application.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Contributions;

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

        public bool Exists(LocationCode locationCode)
        {
            return _locationDefinitionCatalog.Exists(locationCode);
        }

        private static LocationContribution MapToLocationContribution(ReferenceLocationDefinition location)
        {
            return new LocationContribution(location.Code, location.Domain);
        }
    }
}
