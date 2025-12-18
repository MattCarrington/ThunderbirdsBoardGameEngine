using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Initialisers
{
    internal class DisasterCardReferenceSourceInitializer
    {
        private readonly IDisasterCardReader _disasterCardReader;

        public DisasterCardReferenceSourceInitializer(IDisasterCardReader disasterCardReader)
        {
            _disasterCardReader = disasterCardReader ?? throw new ArgumentNullException(nameof(disasterCardReader));
        }

        public async Task<InMemoryDisasterCardReferenceSource> InitializeAsync(CancellationToken cancellationToken = default)
        {
            var cards = await _disasterCardReader.GetAllAsync(cancellationToken);

            var snapshot = (cards as ImmutableArray<DisasterCard>?) ?? cards.ToImmutableArray();

            if (snapshot.IsDefaultOrEmpty)
            {
                throw new InvalidDataException("No disaster cards were loaded from the data source.");
            }

            var version = $"card-{cards.Count}";

            return new InMemoryDisasterCardReferenceSource(snapshot, version);
        }
    }
}
