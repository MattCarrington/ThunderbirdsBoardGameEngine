using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Services
{
    internal class CharacterDefinitionService : ICharacterDefinitionService
    {
        private readonly ICharacterDefinitionReferenceSource _characterReferenceSource;

        public CharacterDefinitionService(ICharacterDefinitionReferenceSource characterReferenceSource)
        {
            _characterReferenceSource = characterReferenceSource ?? throw new ArgumentNullException(nameof(characterReferenceSource));
        }

        public ImmutableArray<CharacterDefinition> GetAll()
        {
            return _characterReferenceSource.Characters;
        }
    }
}
