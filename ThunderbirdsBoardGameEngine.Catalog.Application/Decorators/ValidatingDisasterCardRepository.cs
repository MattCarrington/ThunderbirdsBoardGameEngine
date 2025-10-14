using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Validators;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Decorators
{
    public sealed class ValidatingDisasterCardRepository : IDisasterCardRepository
    {
        private readonly IDisasterCardRepository _inner;

        public ValidatingDisasterCardRepository(IDisasterCardRepository inner)
        {
            _inner = inner;
        }

        public async Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            var cards = await _inner.GetAllAsync(cancellationToken);

            DisasterCardValidator.ValidateAll(cards);

            return cards;
        }
    }
}
