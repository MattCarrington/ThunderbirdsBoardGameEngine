using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardRepository _disasterCardRepository;
        
        public DisasterCardService(IDisasterCardRepository disasterCardRepository)
        {
            _disasterCardRepository = disasterCardRepository;
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            var cards = await _disasterCardRepository.GetAllAsync(CancellationToken.None);

            if (cards is null || !cards.Any())
            {
                return [];
            }

            return cards;
        }
    }
}
