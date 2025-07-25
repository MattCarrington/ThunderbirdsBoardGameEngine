using ThunderbirdsBoardGameEngine.GameData.Api.Entities;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Services
{
    public class DisasterCardService
    {
        private readonly IDisasterCardRepository _disasterCardRepository;

        public DisasterCardService(IDisasterCardRepository disasterCardRepository)
        {
            _disasterCardRepository = disasterCardRepository;
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync()
        {
            return await _disasterCardRepository.GetAllAsync();
        }
    }
}
