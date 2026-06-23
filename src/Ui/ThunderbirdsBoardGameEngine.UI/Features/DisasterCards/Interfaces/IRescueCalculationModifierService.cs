using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces
{
    public interface IRescueCalculationModifierService
    {
        IReadOnlyList<CardModifierViewModel> GetEventCards();

        IReadOnlyList<CardModifierViewModel> GetFabCards();
    }
}