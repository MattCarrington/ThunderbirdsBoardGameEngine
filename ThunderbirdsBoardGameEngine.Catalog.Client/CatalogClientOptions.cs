namespace ThunderbirdsBoardGameEngine.Catalog.Client
{
    /// <summary>
    /// Configuration options for the Thunderbirds catalog HTTP client.
    /// </summary>
    /// <remarks>
    /// These options are typically bound from configuration (e.g. <c>appsettings.json</c>)
    /// and used when registering the catalog client via dependency injection.
    /// </remarks>
    public class CatalogClientOptions
    {
        /// <summary>
        /// Gets or sets the absolute base address of the catalog API.
        /// </summary>
        /// <remarks>
        /// This should be an absolute URI, for example: <c>https://api.example.com/</c>.
        /// It is used as the <see cref="HttpClient.BaseAddress"/> for the catalog client.
        /// </remarks>
        public string BaseAddress { get; set; } = string.Empty;
    }
}
