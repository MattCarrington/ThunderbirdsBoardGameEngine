namespace ThunderbirdsBoardGameEngine.Client.Core.Configuration
{
    /// <summary>
    /// Represents configuration options for an API client.
    /// </summary>
    public interface IApiClientOptions
    {
        /// <summary>
        /// Gets or sets the base address of the API client.
        /// </summary>
        string BaseAddress { get; set; }
    }
}
