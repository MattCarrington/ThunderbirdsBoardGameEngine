using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Decorators
{
    public class ValidatingCharacterDefinitionReader : ICharacterDefinitionReader
    {
        private readonly ICharacterDefinitionReader _inner;

        public ValidatingCharacterDefinitionReader(ICharacterDefinitionReader inner)
        {
            _inner = inner ?? throw new ArgumentNullException(nameof(inner));
        }

        public async Task<IReadOnlyList<CharacterDefinition>> GetAllAsync(CancellationToken cancellationToken)
        {
            var characters = await _inner.GetAllAsync(cancellationToken);

            CharacterDefinitionValidator.ValidateAll(characters);

            return characters;
        }
    }
}