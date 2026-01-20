using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources
{
    internal sealed class InMemoryCharacterDefinitionReferenceSource : ICharacterReferenceSource
    {
        public string Version { get; }

        public ImmutableArray<CharacterDefinition> Characters { get; }

        public InMemoryCharacterDefinitionReferenceSource(ImmutableArray<CharacterDefinition> characters, string version)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(version);

            if (characters.IsDefaultOrEmpty)
            {
                throw new ArgumentException("There must be at least one character definition", nameof(characters));
            }

            Characters = characters;
            Version = version;
        }
    }
}
