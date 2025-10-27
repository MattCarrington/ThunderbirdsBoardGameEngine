using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Catalogs
{
    internal sealed class InMemoryDisasterCardCatalog : IDisasterCardCatalog
    {
        public InMemoryDisasterCardCatalog(ImmutableArray<DisasterCard> disasterCards, string version)
        {
            ArgumentNullException.ThrowIfNull(disasterCards);
            ArgumentException.ThrowIfNullOrWhiteSpace(version);

            if (disasterCards.IsDefaultOrEmpty)
            {
                throw new ArgumentException("There must be at least one disaster card", nameof(disasterCards));
            }

            Version = version;
            Cards = disasterCards;
        }

        public string Version { get; }

        public ImmutableArray<DisasterCard> Cards { get; }
    }
}
