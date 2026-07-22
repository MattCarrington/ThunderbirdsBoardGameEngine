using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class RocketMalfunction : IMovementSpeedModifierSource
    {
        public CardCode EventCardCode => KnownEventCardCodes.RocketMalfunction;

        public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
        {
            if (input == KnownThunderbirdCodes.Thunderbird3)
            {
                return new AppliedMovementSpeedModifier(
                    Card: KnownEventCardCodes.RocketMalfunction,
                    EffectiveTopSpeed: 1,
                    Message: "Rocket Malfunction: Thunderbird 3's top speed is reduced to 1.");
            }

            return null;
        }
    }
}
