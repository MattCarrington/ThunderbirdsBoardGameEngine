using ThunderbirdsBoardGameEngine.Catalog.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.UI.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class CharactersService : ICharactersService
    {
        private readonly ICharactersClient _client;

        public CharactersService(ICharactersClient client)
        {
            _client = client;
        }

        public async Task<IReadOnlyList<CharacterDto>> GetAllAsync()
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
