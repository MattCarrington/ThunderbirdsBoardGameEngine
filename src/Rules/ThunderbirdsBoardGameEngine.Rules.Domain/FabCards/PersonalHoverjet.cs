using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class PersonalHoverjet : IRescueModifierSource
    {
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType.Land)
            {
                yield return new AppliedRescueModifier
                {
                    Key = "personal-hoverjet",
                    Value = 3,
                    SourceType = SourceType.FabCard
                };
            }
        }
    }
}
