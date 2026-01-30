using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeCharacterDefinitionReferenceSource : ICharacterDefinitionReferenceSource
    {
        private readonly FrozenDictionary<Character, CharacterDefinition> _definitions;

        public string Version => "test";

        public ImmutableArray<CharacterDefinition> Characters => _definitions.Values;

        public FakeCharacterDefinitionReferenceSource()
        {
            _definitions = TestCharacters.ValidSix.ToFrozenDictionary(c => c.Key);
        }

        public CharacterDefinition GetCharacterDefinition(Character character)
        {
            return _definitions[character];
        }
    }
}
