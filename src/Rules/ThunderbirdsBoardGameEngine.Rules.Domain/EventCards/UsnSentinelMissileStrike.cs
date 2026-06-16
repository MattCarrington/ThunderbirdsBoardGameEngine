using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class UsnSentinelMissileStrike : IMovementSpeedModifierSource
    {
        public CardCode EventCardCode => KnownEventCardCodes.UsnSentinelMissileStrike;

        public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
        {
            if (input == KnownThunderbirdCodes.Thunderbird2)
            {
                return new AppliedMovementSpeedModifier(
                    Card: EventCardCode,
                    TopSpeedModifier: 1,
                    Message: "USN Sentinel Missile Strike: Thunderbird 2's top speed is reduced to 1.");
            }

            return null;
        }
    }
}
