using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Catalogs;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initialisers
{
    internal class DisasterCardCatalogInitializer
    {
        private readonly IDisasterCardReader _disasterCardReader;

        public DisasterCardCatalogInitializer(IDisasterCardReader disasterCardReader)
        {
            _disasterCardReader = disasterCardReader ?? throw new ArgumentNullException(nameof(disasterCardReader));
        }

        public async Task<InMemoryDisasterCardCatalog> InitializeAsync(CancellationToken cancellationToken = default)
        {
            var cards = await _disasterCardReader.GetAllAsync(cancellationToken);

            var snapshot = (cards as ImmutableArray<DisasterCard>?) ?? cards.ToImmutableArray();

            if (snapshot.IsDefaultOrEmpty)
            {
                throw new InvalidDataException("No disaster cards were loaded from the data source.");
            }

            var version = $"card-{cards.Count}";

            return new InMemoryDisasterCardCatalog(snapshot, version);
        }
    }
}
