using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardReferenceSource _disasterCardReferenceSource;

        public DisasterCardService(IDisasterCardReferenceSource disasterCardReferenceSource)
        {
            _disasterCardReferenceSource = disasterCardReferenceSource ?? throw new ArgumentNullException(nameof(disasterCardReferenceSource));
        }

        public ImmutableArray<DisasterCard> GetAll()
        {
            return _disasterCardReferenceSource.Cards;
        }
    }
}
