using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.FabCards;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Infrastructure.Registries
{
    internal class BonusModifierSourceRegistry : IBonusModifierSourceRegistry
    {
        private readonly Dictionary<CardCode, IBonusModifierSource> _registry = new()
        {
            [new CardCode("remote-control-hover-camera")] = new RemoteControlHoverCamera(),
            [new CardCode("personal-hoverjet")] = new PersonalHoverjet(),
            [new CardCode("underwater-sealing-unit")] = new UnderwaterSealingUnit(),
            [new CardCode("astronaut-spacewalk")] = new AstronautSpacewalk()
        };

        public bool TryGetBonusModifierSource(CardCode cardCode, out IBonusModifierSource? source)
        {
            return _registry.TryGetValue(cardCode, out source!);
        }
    }
}
