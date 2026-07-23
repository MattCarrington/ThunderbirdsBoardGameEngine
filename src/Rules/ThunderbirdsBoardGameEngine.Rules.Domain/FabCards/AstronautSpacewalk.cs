using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class AstronautSpacewalk : ICardRescueModifierSource
    {
        public CardCode CardCode => KnownFabCardCodes.AstronautSpacewalk;

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
