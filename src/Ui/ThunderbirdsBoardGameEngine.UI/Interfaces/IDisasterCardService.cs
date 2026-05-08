using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Interfaces
{
    public interface IDisasterCardService
    {
        IReadOnlyList<DisasterCardViewModel> GetAll();

        DisasterCardViewModel GetByCode(string code);
    }
}