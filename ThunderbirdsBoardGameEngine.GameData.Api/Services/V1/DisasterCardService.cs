using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Api.Interfaces.V1;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Services.V1
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardRepository _disasterCardRepository;
        
        public DisasterCardService(IDisasterCardRepository disasterCardRepository)
        {
            _disasterCardRepository = disasterCardRepository;
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync()
        {
            var cards = await _disasterCardRepository.GetAllAsync();

            if (cards is null || !cards.Any())
            {
                return [];
            }

            return cards;
        }

        public async Task<DisasterCard> GetByIdAsync(int id)
        {
            var card = await _disasterCardRepository.GetByIdAsync(id);

            if (card is null)
            {
                return null;
            }

            return card;
        }
    }
}
