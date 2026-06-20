using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces
{
    public interface ICharacterService
    {
        IReadOnlyList<CharacterViewModel> GetAll();
    }
}