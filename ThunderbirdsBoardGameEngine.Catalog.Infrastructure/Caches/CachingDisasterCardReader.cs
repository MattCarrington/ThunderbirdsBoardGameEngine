using System.Collections.Immutable;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Caches
{
    internal sealed class CachingDisasterCardReader : IDisasterCardReader
    {
        private readonly IDisasterCardReader _inner;
        private readonly Lazy<Task<IReadOnlyList<DisasterCard>>> _cache;

        public CachingDisasterCardReader(IDisasterCardReader inner)
        {
            _inner = inner;

            _cache = new Lazy<Task<IReadOnlyList<DisasterCard>>>(async () =>
            {
                var cards = await _inner.GetAllAsync(CancellationToken.None);

                ImmutableArray<DisasterCard> snapshot = [.. cards]; 

                return snapshot;
            }, LazyThreadSafetyMode.ExecutionAndPublication);
        }

        public Task<IReadOnlyList<DisasterCard>> GetAllAsync(CancellationToken cancellationToken)
        {
            return _cache.Value;
        }
    }
}
