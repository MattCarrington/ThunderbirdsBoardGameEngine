using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public interface ICardBonusModifierSourceRegistry
    {
        bool TryGetBonusModifierSource(CardCode cardCode, out ICardRescueModifierSource source);
    }
}