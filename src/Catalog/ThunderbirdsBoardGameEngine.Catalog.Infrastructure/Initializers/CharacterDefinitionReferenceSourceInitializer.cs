using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initializers
{
    internal class CharacterDefinitionReferenceSourceInitializer : ICharacterDefinitionReferenceSourceInitializer
    {
        private readonly ICharacterReader _characterReader;

        public CharacterDefinitionReferenceSourceInitializer(ICharacterReader characterReader)
        {
            _characterReader = characterReader ?? throw new ArgumentNullException(nameof(characterReader));
        }

        public async Task<InMemoryCharacterDefinitionReferenceSource> InitializeAsync(CancellationToken cancellationToken = default)
        {
            var characters = await _characterReader.GetAllAsync(cancellationToken);

            var snapshot = (characters as ImmutableArray<CharacterDefinition>?) ?? characters.ToImmutableArray();

            if (snapshot.IsDefaultOrEmpty)
            {
                throw new InvalidDataException("No character definitions were found.");
            }

            var version = $"Characters+{DateTime.UtcNow:yyyyMMddHHmmss}";

            return new InMemoryCharacterDefinitionReferenceSource(snapshot, version);
        }
    }
}
