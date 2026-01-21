using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface ICharacterDefinitionReader
    {
        Task<IReadOnlyList<CharacterDefinition>> GetAllAsync(CancellationToken cancellationToken);
    }
}
