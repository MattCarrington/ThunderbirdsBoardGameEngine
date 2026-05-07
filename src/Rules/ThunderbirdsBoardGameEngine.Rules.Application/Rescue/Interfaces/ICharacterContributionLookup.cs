using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    /// <summary>
    /// Provides a mechanism for retrieving contribution information associated with a specific character code.
    /// </summary>
    public interface ICharacterContributionLookup
    {
        /// <summary>
        /// Retrieves the contribution details for the specified character code.
        /// </summary>
        /// <param name="characterCode">The code representing the character for which to obtain contribution information.</param>
        /// <returns>A CharacterContribution object containing details about the specified character's contribution.</returns>
        CharacterContribution GetCharacterContribution(CharacterCode characterCode);
    }
}
