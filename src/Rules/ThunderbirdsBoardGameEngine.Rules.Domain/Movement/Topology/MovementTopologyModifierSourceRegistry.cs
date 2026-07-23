using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public sealed class MovementTopologyModifierSourceRegistry : IMovementTopologyModifierSourceRegistry
    {
        private readonly FrozenDictionary<CardCode, IMovementTopologyModifierSource> _sources;

        public MovementTopologyModifierSourceRegistry(IEnumerable<IMovementTopologyModifierSource> sources)
        {
            ArgumentNullException.ThrowIfNull(sources, nameof(sources));

            if (sources.Any(source => source == null))
            {
                throw new ArgumentException("Movement topology modifier sources cannot contain null values.", nameof(sources));
            }

            _sources = sources.ToFrozenDictionary(source => source.CardCode);
        }

        public bool TryGetEventCard(
            CardCode cardCode,
            [NotNullWhen(true)] out IMovementTopologyModifierSource? source)
        {
            return _sources.TryGetValue(cardCode, out source);
        }
    }
}
