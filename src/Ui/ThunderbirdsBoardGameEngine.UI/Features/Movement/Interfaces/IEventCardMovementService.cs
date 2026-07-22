using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces
{
    public interface IEventCardMovementService
    {
        IReadOnlyList<CardModifierViewModel> GetSpeedModificationEventCards();
    }
}