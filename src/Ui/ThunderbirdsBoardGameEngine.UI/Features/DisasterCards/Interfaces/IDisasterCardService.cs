using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces
{
    public interface IDisasterCardService
    {
        IReadOnlyList<DisasterCardViewModel> GetAll();

        DisasterCardViewModel GetByCode(string code);
    }
}