using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed class RescueCalculationInput
    {
        public IReadOnlyCollection<DisasterBonusKey> AppliedBonusKeys { get; }

        public RescueCalculationInput(IReadOnlyCollection<DisasterBonusKey> disasterBonusKeys)
        {
             AppliedBonusKeys = disasterBonusKeys;
        }
    }
}
