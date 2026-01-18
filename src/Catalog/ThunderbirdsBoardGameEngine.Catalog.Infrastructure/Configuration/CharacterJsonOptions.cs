namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration
{
    /// <summary>
    /// Represents configuration options for accessing a character JSON file.
    /// </summary>
    public sealed class CharacterJsonOptions
    {
        /// <summary>
        /// Gets or sets the file system path to the character JSON file.
        /// </summary>
        /// <remarks>
        /// This can be an absolute or relative path, depending on how the application
        /// is deployed and configured.
        /// </remarks>
        public string FilePath { get; set; } = string.Empty;
    }
}
