using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1
{
    public interface IDisasterCardsClient
    {
        Task<ApiResult<IReadOnlyList<DisasterCardDto>>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}