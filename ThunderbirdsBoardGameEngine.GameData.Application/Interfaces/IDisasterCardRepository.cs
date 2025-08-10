using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Application.Interfaces
{
    public interface IDisasterCardRepository
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();
        Task<DisasterCard> GetByIdAsync(int id);
    }
}