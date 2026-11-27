using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.DetermineFabCardBonus
{
    public record DetermineFabCardBonusResponse(CardCode FabCardCode, int BonusValue)
    {
    }
}
