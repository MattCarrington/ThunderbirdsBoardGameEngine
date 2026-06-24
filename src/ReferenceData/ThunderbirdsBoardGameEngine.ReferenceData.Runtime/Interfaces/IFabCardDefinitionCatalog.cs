using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces
{
    /// <summary>
    /// Provides read-only access to fab card definitions from the reference data snapshot.
    /// </summary>
    public interface IFabCardDefinitionCatalog
    {
        /// <summary>
        /// Determines whether a card with the specified code exists.
        /// </summary>
        /// <param name="cardCode">The code of the card to locate. Cannot be null.</param>
        /// <returns>true if a card with the specified code exists; otherwise, false.</returns>
        bool Exists(CardCode cardCode);
    }
}