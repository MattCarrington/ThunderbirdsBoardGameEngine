using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators
{
    public sealed class CardUniquenessValidator : ISnapshotValidator
    {
        public void Validate(ReferenceDataSnapshot snapshot)
        {
            var allCards = snapshot.FabCardDefinitions.Select(c => new CardEntry(c.Code.Value, c.DisplayName, "F.A.B. Card"))
                .Concat(snapshot.EventCardDefinitions.Select(c => new CardEntry(c.Code.Value, c.DisplayName, "Event Card")))
                .Concat(snapshot.DisasterDefinitions.Select(d => new CardEntry(d.Code.Value, d.DisplayName, "Disaster Card")))
                .ToList();

            EnsureUniqueValues(allCards, c => c.Code, "codes");
            EnsureUniqueValues(allCards, c => c.Name, "names");
        }

        private static void EnsureUniqueValues<T>(
            IReadOnlyList<T> items,
            Func<T, string> selector,
            string entityDescription)
        {
            var duplicates = items
                .GroupBy(selector)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicates.Count != 0)
            {
                throw new ReferenceDataCompilationException(
                    $"Card {entityDescription} must be unique across card types to prevent ambiguous card references. " +
                    $"Duplicate {entityDescription} found: {string.Join(", ", duplicates)}");
            }
        }

        private sealed record CardEntry(
            string Code,
            string Name,
            string CardType
        );
    }
}