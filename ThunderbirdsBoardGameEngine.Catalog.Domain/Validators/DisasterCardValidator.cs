using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Validators
{
    public static class DisasterCardValidator
    {
        public static void ValidateAll(IEnumerable<DisasterCard> cards)
        {
            var ids = new HashSet<int>();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            ArgumentNullException.ThrowIfNull(cards);

            if (cards.Any(c => c == null))
            {
                throw new DisasterCardValidationException("Disaster Cards collection contains null entries.");
            }

            foreach (var card in cards)
            {
                if (!ids.Add(card.Id))
                {
                    throw new DisasterCardValidationException($"Duplicate Disaster Card Id found: {card.Id}");
                }

                if (!names.Add(card.Name))
                {
                    throw new DisasterCardValidationException($"Duplicate Disaster Card Name found: {card.Name}");
                }

                if (!codes.Add(card.Code))
                {
                    throw new DisasterCardValidationException($"Duplicate Disaster Card Code found: {card.Code}");
                }
            }
        }
    }
}
