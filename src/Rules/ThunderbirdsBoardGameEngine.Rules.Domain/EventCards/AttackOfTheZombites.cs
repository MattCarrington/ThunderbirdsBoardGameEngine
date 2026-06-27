using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class AttackOfTheZombites : IMovementSpeedModifierSource
    {
        public CardCode EventCardCode => KnownEventCardCodes.AttackOfTheZombites;

        public AppliedMovementSpeedModifier? ApplyMovementModifier(ThunderbirdCode input)
        {
            if (input == KnownThunderbirdCodes.Thunderbird1)
            {
                return new AppliedMovementSpeedModifier(
                    Card: KnownEventCardCodes.AttackOfTheZombites,
                    EffectiveTopSpeed: 1,
                    Message: "Attack of the Zombites: Thunderbird 1's top speed is reduced to 1.");
            }

            return null;
        }
    }
}
