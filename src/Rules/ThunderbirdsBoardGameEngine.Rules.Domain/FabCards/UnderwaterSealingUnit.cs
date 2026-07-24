using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class UnderwaterSealingUnit : ICardRescueModifierSource
    {
        public CardCode CardCode => KnownFabCardCodes.UnderwaterSealingUnit;

        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType.Sea)
            {
                yield return new AppliedRescueModifier
                {
                    Key = CardCode.Value,
                    Value = 3,
                    SourceType = SourceType.FabCard
                };
            }
        }
    }
}
