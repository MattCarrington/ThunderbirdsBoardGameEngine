using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Validators
{
    public static class DisasterCardValidator
    {
        public static void ValidateAll(IEnumerable<DisasterCard> cards)
        {
            ArgumentNullException.ThrowIfNull(cards);

            var list = cards.ToList();

            var ids = new HashSet<int>();
            var names = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            var codes = new HashSet<string>(StringComparer.OrdinalIgnoreCase);

            if (list.Contains(null!))
            {
                throw DisasterCardValidationException.NullEntry();
            }

            foreach (var card in list)
            {
                if (!ids.Add(card.Id))
                {
                    throw DisasterCardValidationException.DuplicateId(card.Id, card.Name);
                }

                if (!names.Add(card.Name))
                {
                    throw DisasterCardValidationException.DuplicateName(card.Id, card.Name);
                }

                if (!codes.Add(card.Code))
                {
                    throw DisasterCardValidationException.DuplicateCode(card.Id, card.Name, card.Code);
                }
            }
        }
    }
}
