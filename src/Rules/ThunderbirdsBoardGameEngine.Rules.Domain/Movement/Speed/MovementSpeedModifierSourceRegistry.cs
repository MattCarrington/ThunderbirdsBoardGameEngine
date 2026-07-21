using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed
{
    public sealed class MovementSpeedModifierSourceRegistry : IMovementSpeedModifierSourceRegistry
    {
        private readonly FrozenDictionary<CardCode, IMovementSpeedModifierSource> _sources;

        public MovementSpeedModifierSourceRegistry(
            IEnumerable<IMovementSpeedModifierSource> sources)
        {
            _sources = sources.ToFrozenDictionary(x => x.EventCardCode);
        }

        public bool TryGetEventCard(
            CardCode cardCode,
            [NotNullWhen(true)] out IMovementSpeedModifierSource? source)
        {
            return _sources.TryGetValue(cardCode, out source);
        }
    }
}
