using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.ReferenceSources
{
    internal sealed class InMemoryDisasterCardReferenceSource : IDisasterCardReferenceSource, IDisasterCardReferenceSourceProbe
    {
        FrozenDictionary<int, DisasterCard> _cards;

        public InMemoryDisasterCardReferenceSource(ImmutableArray<DisasterCard> disasterCards, string version)
        {
            ArgumentNullException.ThrowIfNull(disasterCards);
            ArgumentException.ThrowIfNullOrWhiteSpace(version);

            if (disasterCards.IsDefaultOrEmpty)
            {
                throw new ArgumentException("There must be at least one disaster card", nameof(disasterCards));
            }

            Version = version;
            Cards = disasterCards;

            _cards = disasterCards.ToFrozenDictionary(c => c.Id);
        }

        public string Version { get; }

        public ImmutableArray<DisasterCard> Cards { get; }

        public int Count => Cards.Length;

        public DisasterCard GetById(int id)
        {
            if (!_cards.TryGetValue(id, out var card))
            {
                throw new DisasterCardNotFoundException(id);
            }

            return card;
        }
    }
}
