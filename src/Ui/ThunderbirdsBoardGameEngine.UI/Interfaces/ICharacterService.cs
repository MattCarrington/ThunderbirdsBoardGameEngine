using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Interfaces
{
    public interface ICharacterService
    {
        IReadOnlyList<CharacterViewModel> GetAll();
    }
}