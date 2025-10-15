using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardReader _disasterCardRepository;
        
        public DisasterCardService(IDisasterCardReader disasterCardRepository)
        {
            _disasterCardRepository = disasterCardRepository ?? throw new ArgumentNullException(nameof(disasterCardRepository));
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            return await _disasterCardRepository.GetAllAsync(cancellationToken);
        }
    }
}
