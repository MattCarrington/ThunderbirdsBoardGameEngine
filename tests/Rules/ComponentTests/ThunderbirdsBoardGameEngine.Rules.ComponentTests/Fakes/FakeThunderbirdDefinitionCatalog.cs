using System.Collections.Frozen;
using System.Collections.Immutable;
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

        public ReferenceThunderbirdDefinition GetByCode(ThunderbirdCode code)
        {
            return _thunderbirds[code];
        }
    }

}
