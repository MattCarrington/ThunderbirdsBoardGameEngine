using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources
{
    internal sealed class InMemoryCharacterDefinitionReferenceSource : ICharacterDefinitionReferenceSource, ICharacterDefinitionReferenceSourceProbe
    {
        private readonly FrozenDictionary<Character, CharacterDefinition> _characters;

        private readonly IReadOnlyCollection<Character> _keys;

        public string Version { get; }

        public ImmutableArray<CharacterDefinition> Characters { get; }

        public IReadOnlyCollection<Character> Keys => _keys;

        public InMemoryCharacterDefinitionReferenceSource(ImmutableArray<CharacterDefinition> characters, string version)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(version);

            if (characters.IsDefaultOrEmpty)
            {
                throw new ArgumentException("There must be at least one character definition", nameof(characters));
            }

            Characters = characters;
            Version = version;

            _characters = characters.ToFrozenDictionary(c => c.Key);

            _keys = characters.Select(c => c.Key).ToImmutableArray();
        }

        public CharacterDefinition GetCharacterDefinition(Character character)
        {
            if (!_characters.TryGetValue(character, out var definition))
            {
                throw new CharacterDefinitionNotFoundException(character);
            }

            return definition;
        }
    }
}
