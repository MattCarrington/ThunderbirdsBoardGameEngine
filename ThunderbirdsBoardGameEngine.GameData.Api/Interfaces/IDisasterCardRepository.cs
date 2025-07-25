using ThunderbirdsBoardGameEngine.GameData.Api.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Interfaces
{
    public interface IDisasterCardRepository
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();
    }
}