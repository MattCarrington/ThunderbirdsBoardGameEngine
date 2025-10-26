using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Catalogs
{
    internal sealed class InMemoryDisasterCardCatalog : IDisasterCardCatalog
    {
        public InMemoryDisasterCardCatalog(IReadOnlyList<DisasterCard> disasterCards, string version)
        {
            ArgumentNullException.ThrowIfNull(disasterCards);
            ArgumentException.ThrowIfNullOrWhiteSpace(version);

            if (disasterCards.Count == 0)
            {
                throw new ArgumentException("There must be at least one disaster card", nameof(disasterCards));
            }

            Version = version;
            All = disasterCards.ToImmutableArray();
        }

        public string Version { get; }

        public IReadOnlyList<DisasterCard> All { get; }
    }
}
