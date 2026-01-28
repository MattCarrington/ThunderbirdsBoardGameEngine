using ThunderbirdsBoardGameEngine.PublishedLanguage.Characters;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public class CharacterContribution : IBonusModifierSource
    {
        public CharacterCode Key { get; set; }

        public CharacterRescueBonusContribution? RescueBonusContribution { get; set; }

        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (RescueBonusContribution is null)
            {
                yield break;
            }

            if (RescueBonusContribution.RescueType == input.RescueType)
            {
                yield return new AppliedRescueModifier
                {
                    SourceType = "character",
                    Key = Key.ToString(),
                    Value = RescueBonusContribution.BonusValue
                };
            }
        }
    }

    public class CharacterRescueBonusContribution
    {
        public RescueType RescueType { get; set; }

        public int BonusValue { get; set; }
    }
}
