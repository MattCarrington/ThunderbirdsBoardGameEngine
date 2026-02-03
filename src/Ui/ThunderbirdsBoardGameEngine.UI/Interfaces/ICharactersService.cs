using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.UI.Interfaces
{
    public interface ICharactersService
    {
        Task<IReadOnlyList<CharacterDto>> GetAllAsync();
    }
}