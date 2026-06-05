using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal class ThunderbirdDefinitionCatalog : IThunderbirdDefinitionCatalog
    {
        private readonly FrozenDictionary<ThunderbirdCode, ReferenceThunderbirdDefinition> _byCode;

        public ThunderbirdDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _byCode = snapshot.ThunderbirdDefinitions
                .ToDictionary(t => t.Code)
                .ToFrozenDictionary();
        }

        public ReferenceThunderbirdDefinition GetByCode(ThunderbirdCode code)
        {
            if (!_byCode.TryGetValue(code, out var thunderbird))
            {
                throw new KeyNotFoundException($"Thunderbird with code '{code}' not found.");
            }

            return thunderbird;
        }
    }
}
