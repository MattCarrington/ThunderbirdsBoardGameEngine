using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Calculation;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class UnderwaterSealingUnit : IFabCardRescueModifierSource
    {
        public CardCode CardCode => KnownFabCardCodes.UnderwaterSealingUnit;

        public RescueType RescueType => RescueType.Sea;

        public int BonusValue => 3;

        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType)
            {
                yield return new AppliedRescueModifier
                {
                    Key = CardCode.Value,
                    Value = BonusValue,
                    SourceType = SourceType.FabCard
                };
            }
        }
    }
}
