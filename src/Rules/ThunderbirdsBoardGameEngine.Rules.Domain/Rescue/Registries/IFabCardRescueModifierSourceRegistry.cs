using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Registries
{
    public interface IFabCardRescueModifierSourceRegistry
    {
        bool TryGetCard(RescueType rescueType, out IFabCardRescueModifierSource source);
    }
}