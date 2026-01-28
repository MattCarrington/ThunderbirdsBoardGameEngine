using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed class RescueCalculationInput
    {
        public IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys { get; }

        public RescueType RescueType { get; }

        public RescueCalculationInput(IReadOnlyCollection<DisasterBonusKey> presentDisasterBonusKeys, RescueType rescueType)
        {
            PresentDisasterBonusKeys = presentDisasterBonusKeys;
            RescueType = rescueType;
        }
    }
}
