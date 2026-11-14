using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces
{
    public interface IThunderbirdMovementOptionsService
    {
        IReadOnlyList<SelectionOptionViewModel> GetAllMobileVehicles();
    }
}