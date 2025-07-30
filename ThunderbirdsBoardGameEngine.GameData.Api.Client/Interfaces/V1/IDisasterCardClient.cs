using ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.Interfaces.V1
{
    public interface IDisasterCardClient
    {
        Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync();

        Task<ApiResult<DisasterCardDto>> GetByIdAsync(int id);
    }
}