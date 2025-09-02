using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardsClient _client;

        public DisasterCardService(IDisasterCardsClient client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<DisasterCardDto>> GetAllAsync()
        {
            var result = await _client.GetAllAsync();

            if (result.Success && result.Data is not null)
            {
                return result.Data;
            }

            return []; // Return an empty list if the request fails
        }
    }
}
