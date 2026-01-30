using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    /// <summary>
    /// Provides a mechanism for retrieving disaster contribution information based on a specified disaster code.
    /// </summary>
    public interface IDisasterContributionLookup
    {
        /// <summary>
        /// Retrieves the disaster contribution associated with the specified disaster code.
        /// </summary>
        /// <param name="disasterCode">The code that uniquely identifies the disaster for which to retrieve the contribution.</param>
        /// <returns>A <see cref="DisasterContribution"/> object representing the contribution for the specified disaster code,
        /// or <see langword="null"/> if no contribution is found.</returns>
        DisasterContribution GetDisasterContribution(CardCode disasterCode);
    }
}
