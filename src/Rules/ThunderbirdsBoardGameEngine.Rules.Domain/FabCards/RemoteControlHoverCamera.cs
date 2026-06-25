using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.FabCards
{
    public sealed class RemoteControlHoverCamera : IRescueModifierSource
    {
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            if (input.RescueType == RescueType.Air)
            {
                yield return new AppliedRescueModifier
                {
                    Key = "remote-control-hover-camera",
                    Value = 3,
                    SourceType = SourceType.FabCard
                };
            }
        }
    }
}
