namespace ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces
{
    /// <summary>
    /// Provides diagnostic access to the disaster card catalog.
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
    public interface IDisasterCardCatalogProbe
    {
        /// <summary>
        /// Gets the version of the catalog.
        /// </summary>
        string Version { get; }

        /// <summary>
        /// Gets the number of the disaster cards in the catalog.
        /// </summary>
        int Count { get; }
    }
}
