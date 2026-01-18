using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface ICharacterReader
    {
        Task<IReadOnlyList<CharacterDefinition>> GetAllAsync(CancellationToken cancellationToken);
    }
}
