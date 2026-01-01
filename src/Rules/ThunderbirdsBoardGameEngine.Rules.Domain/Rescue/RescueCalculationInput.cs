using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed class RescueCalculationInput
    {
        public IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys { get; }

        public RescueCalculationInput(IReadOnlyCollection<DisasterBonusKey> presentDisasterBonusKeys)
        {
             PresentDisasterBonusKeys = presentDisasterBonusKeys;
        }
    }
}
