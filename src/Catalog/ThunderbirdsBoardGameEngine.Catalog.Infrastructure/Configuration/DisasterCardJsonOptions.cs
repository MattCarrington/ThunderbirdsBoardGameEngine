namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration
{
    /// <summary>
    /// Configuration options for the disaster card JSON data source.
    /// </summary>
    /// <remarks>
    /// These options are typically bound from configuration under
    /// the <c>Catalog:DisasterCards:Json</c> section.
    /// </remarks>
    public sealed class DisasterCardJsonOptions
    {
        /// <summary>
        /// Gets or sets the file system path to the disaster card JSON file.
        /// </summary>
        /// <remarks>
        /// This can be an absolute or relative path, depending on how the application
        /// is deployed and configured.
        /// </remarks>
        public string FilePath { get; set; } = string.Empty;
    }
}
