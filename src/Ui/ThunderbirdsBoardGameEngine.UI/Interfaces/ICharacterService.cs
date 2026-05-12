using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Interfaces
{
    public interface ICharacterService
    {
        IReadOnlyList<CharacterViewModel> GetAll();
    }
}