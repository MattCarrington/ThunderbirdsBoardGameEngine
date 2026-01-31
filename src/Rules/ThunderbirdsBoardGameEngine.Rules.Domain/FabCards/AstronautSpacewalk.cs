using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class AstronautSpacewalk : IBonusModifierSource
    {
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType.Space)
            {
                yield return new AppliedRescueModifier
                {
                    Key = "astronaut-spacewalk",
                    Value = 3,
                    SourceType = SourceType.FabCard
                };
            }
        }
    }
}
