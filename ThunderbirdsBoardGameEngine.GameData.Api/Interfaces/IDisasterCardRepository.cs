using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Interfaces
{
    public interface IDisasterCardRepository
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();
    }
}