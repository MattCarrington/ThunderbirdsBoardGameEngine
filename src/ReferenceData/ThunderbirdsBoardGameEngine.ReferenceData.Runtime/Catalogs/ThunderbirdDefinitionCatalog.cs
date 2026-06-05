using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
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

        public bool TryGetByCode(ThunderbirdCode code, [NotNullWhen(true)] out ReferenceThunderbirdDefinition? definition)
        {
            return _byCode.TryGetValue(code, out definition);
        }
    }
}
