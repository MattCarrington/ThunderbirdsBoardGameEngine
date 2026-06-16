using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class RocketMalfunction : IMovementModifierSource
    {
        public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
        {
            if (input == KnownThunderbirdCodes.Thunderbird3)
            {
                return new AppliedMovementSpeedModifier(
                    Card: KnownEventCardCodes.RocketMalfunction,
                    TopSpeedModifier: 1,
                    Message: "Rocket Malfunction: Thunderbird 3's top speed is reduced to 1.");
            }

            return null;
        }
    }
}
