using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces
{
    public interface ICharacterService
    {
        IReadOnlyList<CharacterViewModel> GetAll();
    }
}