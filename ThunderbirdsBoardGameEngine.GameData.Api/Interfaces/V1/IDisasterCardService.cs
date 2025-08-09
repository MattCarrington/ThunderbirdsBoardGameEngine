using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Interfaces.V1
{
    public interface IDisasterCardService
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();

        Task<DisasterCard> GetByIdAsync(int id);
    }
}