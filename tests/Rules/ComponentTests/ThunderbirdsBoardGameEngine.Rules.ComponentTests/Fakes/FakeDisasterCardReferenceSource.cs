using System.Collections.Frozen;
using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Rules.ComponentTests.Fakes
{
    public class FakeDisasterCardReferenceSource : IDisasterCardReferenceSource
    {
        private readonly FrozenDictionary<CardCode, DisasterCard> _cards;

        public string Version => "test";

        public ImmutableArray<DisasterCard> Cards => _cards.Values.ToImmutableArray();

        public FakeDisasterCardReferenceSource(params DisasterCard[] disasterCards)
        {
            _cards = disasterCards.ToFrozenDictionary(card => card.Code);
        }

        public DisasterCard GetByCode(CardCode disasterCardCode)
        {
            return _cards[disasterCardCode];
        }
    }
}
