using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class AstronautSpacewalk : IFabCardRescueModifierSource
    {
        public CardCode CardCode => KnownFabCardCodes.AstronautSpacewalk;

        public RescueType RescueType => RescueType.Space;

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
