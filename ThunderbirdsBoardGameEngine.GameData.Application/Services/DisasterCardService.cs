using ThunderbirdsBoardGameEngine.GameData.Application.Interfaces;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.GameData.Application.Services
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
