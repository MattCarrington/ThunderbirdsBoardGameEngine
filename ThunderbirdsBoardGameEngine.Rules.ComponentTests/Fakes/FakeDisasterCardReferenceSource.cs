using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.ComponentTests.Fakes
{
    public class FakeDisasterCardReferenceSource : IDisasterCardReferenceSource
    {
        private readonly FrozenDictionary<int, DisasterCard> _cards;

        public string Version => "test";

        public ImmutableArray<DisasterCard> Cards => _cards.Values.ToImmutableArray();

        public FakeDisasterCardReferenceSource(params DisasterCard[] disasterCards)
        {
             _cards = disasterCards.ToFrozenDictionary(card => card.Id);
        }

        public DisasterCard GetById(int id)
        {
            return _cards[id];
        }
    }
}
