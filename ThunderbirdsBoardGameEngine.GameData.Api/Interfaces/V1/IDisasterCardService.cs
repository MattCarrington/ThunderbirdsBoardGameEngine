using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Interfaces.V1
{
    public interface IDisasterCardService
    {
        Task<IReadOnlyList<DisasterCardDto>> GetAllAsync();
        Task<DisasterCardDto> GetByIdAsync(int id);
    }
}