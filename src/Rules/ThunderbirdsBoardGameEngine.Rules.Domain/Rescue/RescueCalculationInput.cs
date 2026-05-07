using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    /// <summary>
    /// Represents the input parameters required to perform a rescue calculation, including the applicable disaster
    /// bonuses and the type of rescue operation.
    /// </summary>
    public sealed class RescueCalculationInput
    {
        /// <summary>
        /// Gets the collection of disaster bonus keys that are currently present.
        /// </summary>
        /// <remarks>
        /// Only relevant for the determination of bonuses defined on the disaster card
        /// </remarks>
        public IReadOnlyCollection<DisasterBonusKey> PresentDisasterBonusKeys { get; }

        /// <summary>
        /// Gets the type of rescue operation associated with this instance.
        /// </summary>
        /// <remarks>
        /// Not relevant for disaster card bonuses, but may be relevant for other bonus sources
        /// </remarks>
        public RescueType RescueType { get; }

        public RescueCalculationInput(IReadOnlyCollection<DisasterBonusKey> presentDisasterBonusKeys, RescueType rescueType)
        {
            PresentDisasterBonusKeys = presentDisasterBonusKeys;
            RescueType = rescueType;
        }
    }
}
