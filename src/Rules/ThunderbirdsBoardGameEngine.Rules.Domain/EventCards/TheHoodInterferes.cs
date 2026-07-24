using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.KnownIdentities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class TheHoodInterferes : ICardRescueModifierSource
    {
        public CardCode CardCode => KnownEventCardCodes.TheHoodInterferes;

        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            yield return new AppliedRescueModifier
            {
                Key = CardCode.Value,
                Value = -2,
                SourceType = SourceType.EventCard
            };
        }
    }
}
