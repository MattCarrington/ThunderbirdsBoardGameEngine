using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class RemoteControlHoverCamera : ICardRescueModifierSource
    {
        public CardCode CardCode => KnownFabCardCodes.RemoteControlHoverCamera;

        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType.Air)
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
