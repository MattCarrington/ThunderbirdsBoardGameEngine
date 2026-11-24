using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Contributions
{
    public record CharacterRescueBonusContribution(RescueType RescueType, int BonusValue);
}
