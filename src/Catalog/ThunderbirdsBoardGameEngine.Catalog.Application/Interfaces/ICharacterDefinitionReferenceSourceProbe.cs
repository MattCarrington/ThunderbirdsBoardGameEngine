using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Provides diagnostic access to the character definition catalog.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Intended for testing and inspection scenarios where internal catalog
    /// state needs to be observed.
    /// </para>
    /// <para>
    /// This interface should not be used for application logic.
    /// </para>
    /// </remarks>
    public interface ICharacterDefinitionReferenceSourceProbe
    {
        /// <summary>
        /// Gets the version identifier for the current instance.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets a read-only collection containing all characters that are used as keys in the collection.
        /// </summary>
        IReadOnlyCollection<Character> Keys { get; }
    }
}
