using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Shared.Interfaces
{
    public interface ICharacterService
    {
        IReadOnlyList<SelectionOptionViewModel> GetAll();
    }
}