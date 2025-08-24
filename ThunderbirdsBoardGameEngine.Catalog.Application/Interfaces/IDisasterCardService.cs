using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    public interface IDisasterCardService
    {
        Task<IReadOnlyList<DisasterCard>> GetAllAsync();

        Task<DisasterCard> GetByIdAsync(int id);
    }
}