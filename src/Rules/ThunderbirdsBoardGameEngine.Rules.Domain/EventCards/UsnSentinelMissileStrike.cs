using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class UsnSentinelMissileStrike : IMovementSpeedModifierSource
    {
        public CardCode CardCode => KnownEventCardCodes.UsnSentinelMissileStrike;

        public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
        {
            if (input == KnownThunderbirdCodes.Thunderbird2)
            {
                return new AppliedMovementSpeedModifier(
                    Card: CardCode,
                    EffectiveTopSpeed: 1,
                    Message: "USN Sentinel Missile Strike: Thunderbird 2's top speed is reduced to 1.");
            }

            return null;
        }
    }
}
