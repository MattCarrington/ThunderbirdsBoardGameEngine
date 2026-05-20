using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IBonusModifierSourceRegistry
    {
        bool TryGetBonusModifierSource(CardCode cardCode, out IBonusModifierSource? source);
    }
}