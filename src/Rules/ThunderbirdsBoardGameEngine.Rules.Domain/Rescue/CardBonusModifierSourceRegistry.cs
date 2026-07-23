using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class CardBonusModifierSourceRegistry : ICardBonusModifierSourceRegistry
    {
        private readonly FrozenDictionary<CardCode, ICardRescueModifierSource> _registry;

        public CardBonusModifierSourceRegistry(IEnumerable<ICardRescueModifierSource> sources)
        {
            ArgumentNullException.ThrowIfNull(sources, nameof(sources));

            if (sources.Any(source => source == null))
            {
                throw new ArgumentException("Card rescue modifier sources cannot contain null values.", nameof(sources));
            }

            _registry = sources.ToFrozenDictionary(source => source.CardCode);
        }

        public bool TryGetCard(CardCode cardCode, out ICardRescueModifierSource source)
        {
            return _registry.TryGetValue(cardCode, out source!);
        }
    }
}
