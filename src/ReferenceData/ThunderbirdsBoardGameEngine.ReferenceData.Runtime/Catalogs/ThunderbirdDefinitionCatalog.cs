using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Catalogs
{
    internal class ThunderbirdDefinitionCatalog : IThunderbirdDefinitionCatalog
    {
        private readonly FrozenDictionary<ThunderbirdCode, ReferenceThunderbirdDefinition> _byCode;
        private readonly ImmutableArray<ReferenceThunderbirdDefinition> _thunderbirds;

        public ThunderbirdDefinitionCatalog(ReferenceDataSnapshot snapshot)
        {
            ArgumentNullException.ThrowIfNull(snapshot);

            _byCode = snapshot.ThunderbirdDefinitions
                .ToDictionary(t => t.Code)
                .ToFrozenDictionary();

            _thunderbirds = snapshot.ThunderbirdDefinitions.ToImmutableArray();
        }

        public ImmutableArray<ReferenceThunderbirdDefinition> GetAll()
        {
            return _thunderbirds;
        }

        public bool TryGetByCode(ThunderbirdCode code, [NotNullWhen(true)] out ReferenceThunderbirdDefinition? definition)
        {
            return _byCode.TryGetValue(code, out definition);
        }
    }
}
