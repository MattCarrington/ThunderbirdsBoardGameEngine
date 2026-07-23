using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Topology;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class IcelandicVolcanoEruption : IMovementTopologyModifierSource
    {
        public CardCode CardCode => KnownEventCardCodes.IcelandicVolcanoEruption;

        public AppliedMovementTopologyModifier ApplyMovementModifier()
        {
            return new AppliedMovementTopologyModifier(
                Card: CardCode,
                BlockedEdges:
                [
                    new(new LocationCode("europe"), new LocationCode("north-atlantic")),
                    new(new LocationCode("africa"), new LocationCode("north-atlantic")),
                    new(new LocationCode("africa"), new LocationCode("south-atlantic"))
                ],
                Message: "Icelandic Volcano Eruption blocks routes between Europe, Africa and the Atlantic.");
        }
    }
}
