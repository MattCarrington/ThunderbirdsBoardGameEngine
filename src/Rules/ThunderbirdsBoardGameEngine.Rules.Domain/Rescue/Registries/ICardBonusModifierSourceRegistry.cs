using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Registries
{
    public interface ICardBonusModifierSourceRegistry
    {
        bool TryGetCard(CardCode cardCode, out ICardRescueModifierSource source);
    }
}