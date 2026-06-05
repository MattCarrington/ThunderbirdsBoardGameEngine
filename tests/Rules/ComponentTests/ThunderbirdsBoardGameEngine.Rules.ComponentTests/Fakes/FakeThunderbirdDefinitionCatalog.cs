using System.Collections.Frozen;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeThunderbirdDefinitionCatalog : IThunderbirdDefinitionCatalog
    {
        private readonly FrozenDictionary<ThunderbirdCode, ReferenceThunderbirdDefinition> _thunderbirds;

        public FakeThunderbirdDefinitionCatalog(params ReferenceThunderbirdDefinition[] thunderbirds)
        {
            _thunderbirds = thunderbirds.ToFrozenDictionary(thunderbird => thunderbird.Code);
        }

        public bool TryGetByCode(ThunderbirdCode code, [NotNullWhen(true)] out ReferenceThunderbirdDefinition? definition)
        {
            return _thunderbirds.TryGetValue(code, out definition);
        }
    }

}
