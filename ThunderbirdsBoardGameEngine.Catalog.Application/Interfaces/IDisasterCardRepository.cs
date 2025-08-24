using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface IDisasterCardRepository
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();
        Task<DisasterCard> GetByIdAsync(int id);
    }
}