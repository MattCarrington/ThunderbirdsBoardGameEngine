using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces
{
    public interface IRescueCalculationModifierService
    {
        IReadOnlyList<CardModifierViewModel> GetEventCards();

        IReadOnlyList<CardModifierViewModel> GetFabCards();
    }
}