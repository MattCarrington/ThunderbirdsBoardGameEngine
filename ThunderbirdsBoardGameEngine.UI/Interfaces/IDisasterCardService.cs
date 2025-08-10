using ThunderbirdsBoardGameEngine.GameData.Api.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.UI.Interfaces
{
    public interface IDisasterCardService
    {
        Task<IReadOnlyList<DisasterCardDto>> GetAllAsync();
        Task<DisasterCardDto?> GetByIdAsync(int id);
    }
}