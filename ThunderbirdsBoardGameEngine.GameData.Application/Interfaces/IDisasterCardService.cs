using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Application.Interfaces
{
    public interface IDisasterCardService
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();

        Task<DisasterCard> GetByIdAsync(int id);
    }
}