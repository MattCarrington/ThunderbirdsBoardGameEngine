using System.Collections.Frozen;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class FabCardRescueModifierSourceRegistry
    {
        private readonly FrozenDictionary<RescueType, IFabCardRescueModifierSource> _registry;

        public FabCardRescueModifierSourceRegistry(IEnumerable<IFabCardRescueModifierSource> sources)
        {
            ArgumentNullException.ThrowIfNull(sources, nameof(sources));

            if (sources.Any(source => source == null))
            {
                throw new ArgumentException("Fab card rescue modifier sources cannot contain null values.", nameof(sources));
            }

            _registry = sources.ToFrozenDictionary(source => source.RescueType);
        }

        public bool TryGetCard(RescueType rescueType, out IFabCardRescueModifierSource source)
        {
            return _registry.TryGetValue(rescueType, out source!);
        }
    }
}
