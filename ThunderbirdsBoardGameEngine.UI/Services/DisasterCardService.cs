using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardClient _client;

        public DisasterCardService(IDisasterCardClient client)
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

        public async Task<DisasterCardDto?> GetByIdAsync(int id)
        {
            var result = await _client.GetByIdAsync(id);

            if (result.Success && result.Data is not null)
            {
                return result.Data;
            }

            return null; // Return null if the request fails or data is null
        }
    }
}
