using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IBonusModifierSourceRegistry
    {
        IBonusModifierSource? GetBonusModifierSource(CardCode cardCode);
    }
}