using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public interface ICardBonusModifierSourceRegistry
    {
        bool TryGetCard(CardCode cardCode, out ICardRescueModifierSource source);
    }
}