using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class UnderwaterSealingUnit : IBonusModifierSource
    {
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType.Sea)
            {
                yield return new AppliedRescueModifier
                {
                    Key = "underwater-sealing-unit",
                    Value = 3,
                    SourceType = SourceType.FabCard
                };
            }
        }
    }
}
