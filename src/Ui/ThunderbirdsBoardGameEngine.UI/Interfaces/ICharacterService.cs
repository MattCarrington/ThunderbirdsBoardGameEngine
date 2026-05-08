using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Interfaces
{
    public interface ICharacterService
    {
        IReadOnlyList<CharacterViewModel> GetAll();
    }
}