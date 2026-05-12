using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal class LocationDefinitionCatalog : ILocationDefinitionCatalog
    {
        private readonly FrozenDictionary<LocationCode, ReferenceLocationDefinition> _byCode;

        private readonly ImmutableArray<ReferenceLocationDefinition> _locations;

        public LocationDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _byCode = snapshot.LocationDefinitions
                .ToDictionary(l => l.Code)
                .ToFrozenDictionary();

            _locations = snapshot.LocationDefinitions.ToImmutableArray();
        }

        public ImmutableArray<ReferenceLocationDefinition> GetAll()
        {
            return _locations;
        }

        public ReferenceLocationDefinition GetByCode(LocationCode code)
        {
            if (!_byCode.TryGetValue(code, out var location))
            {
                throw new KeyNotFoundException($"No location found with code '{code}'.");
            }

            return location;
        }
    }
}
