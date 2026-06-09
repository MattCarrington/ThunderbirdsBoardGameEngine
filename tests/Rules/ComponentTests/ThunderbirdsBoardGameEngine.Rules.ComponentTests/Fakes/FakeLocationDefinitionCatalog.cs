using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeLocationDefinitionCatalog : ILocationDefinitionCatalog
    {
        private readonly FrozenDictionary<LocationCode, ReferenceLocationDefinition> _locations;

        public FakeLocationDefinitionCatalog(params ReferenceLocationDefinition[] locations)
        {
            _locations = locations.ToFrozenDictionary(location => location.Code);
        }

        public ImmutableArray<ReferenceLocationDefinition> GetAll()
        {
            return _locations.Values;
        }

        public bool Exists(LocationCode code)
        {
            return _locations.ContainsKey(code);
        }

        public ReferenceLocationDefinition GetByCode(LocationCode code)
        {
            return _locations[code];
        }

        public bool TryGetByCode(LocationCode code, [NotNullWhen(true)] out ReferenceLocationDefinition? locationDefinition)
        {
            return _locations.TryGetValue(code, out locationDefinition);
        }
    }
}
