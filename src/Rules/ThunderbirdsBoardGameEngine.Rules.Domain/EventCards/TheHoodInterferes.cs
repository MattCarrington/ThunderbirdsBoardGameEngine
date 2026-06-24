using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.EventCards
{
    public sealed class TheHoodInterferes : IRescueModifierSource
    {
        public IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input)
        {
            yield return new AppliedRescueModifier
            {
                Key = "the-hood-interferes",
                Value = -2,
                SourceType = SourceType.EventCard
            };
        }
    }
}
