using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Interfaces
{
    public interface IDisasterCardService
    {
        Task<IReadOnlyList<DisasterCardDto>> GetAllAsync();
    }
}