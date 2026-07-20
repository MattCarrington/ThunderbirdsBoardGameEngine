using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Validators
{
    public sealed class EventCardValidator : IEventCardValidator
    {
        private readonly IEventCardCatalogLookup _lookup;

        public EventCardValidator(IEventCardCatalogLookup lookup)
        {
            _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
        }

        public void Validate(IReadOnlyCollection<CardCode> eventCards)
        {
            foreach (var card in eventCards)
            {
                if (!_lookup.Exists(card))
                {
                    throw new ReferenceDataNotFoundException("Event Card", card.Value);
                }
            }
        }
    }
}
