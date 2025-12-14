using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Services
{
    public class DisasterCardService : IDisasterCardService
    {
        private readonly IDisasterCardCatalog _disasterCardCatalog;

        public DisasterCardService(IDisasterCardCatalog disasterCardCatalog)
        {
            _disasterCardCatalog = disasterCardCatalog ?? throw new ArgumentNullException(nameof(disasterCardCatalog));
        }

        public ImmutableArray<DisasterCard> GetAll()
        {
            return _disasterCardCatalog.Cards;
        }
    }
}
