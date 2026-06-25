using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeCharacterDefinitionCatalog : ICharacterDefinitionCatalog
    {
        private readonly FrozenDictionary<CharacterCode, ReferenceCharacterDefinition> _characters;

        public FakeCharacterDefinitionCatalog(params ReferenceCharacterDefinition[] characters)
        {
            _characters = characters.ToFrozenDictionary(character => character.Code);
        }

        public ImmutableArray<ReferenceCharacterDefinition> GetAll()
        {
            return _characters.Values.ToImmutableArray();
        }

        public ReferenceCharacterDefinition GetByCode(CharacterCode code)
        {
            return _characters[code];
        }
    }
}
