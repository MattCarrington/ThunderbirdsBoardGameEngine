using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology
{
    public sealed class EffectiveTopographyResolver : IEffectiveTopographyResolver
    {
        private readonly IMovementTopologyModifierSourceRegistry _registry;

        public EffectiveTopographyResolver(IMovementTopologyModifierSourceRegistry registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public EffectiveTopography Resolve(
            Topography baseTopography,
            IReadOnlyCollection<CardCode> activeEventCards)
        {
            ArgumentNullException.ThrowIfNull(baseTopography, nameof(baseTopography));
            ArgumentNullException.ThrowIfNull(activeEventCards, nameof(activeEventCards));

            var modifiers = activeEventCards
                .Select(card => _registry.TryGetEventCard(card, out var source)
                    ? source.ApplyMovementModifier()
                    : null)
                .Where(modifier => modifier is not null)
                .Cast<AppliedMovementTopologyModifier>()
                .ToList();

            return new EffectiveTopography(
                Value: baseTopography.WithoutEdges(modifiers.SelectMany(modifier => modifier.BlockedEdges)),
                Messages: modifiers.Select(modifier => modifier.Message).ToList());
        }
    }
}
