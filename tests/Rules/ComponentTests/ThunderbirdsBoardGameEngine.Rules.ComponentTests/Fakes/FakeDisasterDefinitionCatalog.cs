using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeDisasterDefinitionCatalog : IDisasterDefinitionCatalog
    {
        private readonly FrozenDictionary<CardCode, ReferenceDisasterDefinition> _disasters;

        public FakeDisasterDefinitionCatalog(params ReferenceDisasterDefinition[] disasters)
        {
            _disasters = disasters.ToFrozenDictionary(disaster => disaster.Code);
        }

        public ImmutableArray<ReferenceDisasterDefinition> GetAll()
        {
            return _disasters.Values.ToImmutableArray();
        }

        public ReferenceDisasterDefinition GetByCode(CardCode code)
        {
            return _disasters[code];
        }
    }
}
