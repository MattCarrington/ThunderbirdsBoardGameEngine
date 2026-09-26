using ThunderbirdsBoardGameEngine.Client.Core.Configuration;

namespace ThunderbirdsBoardGameEngine.GameState.Client.Configuration
{
    /// <summary>
    /// Configuration options for the Thunderbirds game state HTTP client.
    /// </summary>
    /// <remarks>
    /// These options are typically bound from configuration (e.g. <c>appsettings.json</c>)
    /// and used when registering the game state client via dependency injection.
    /// </remarks>
    public class GameStateClientOptions : IApiClientOptions
    {
        /// <summary>
        /// Gets the name of the configuration section for the game state client options.
        /// </summary>
        public const string SectionName = "GameStateClient";

        /// <summary>
        /// Gets or sets the absolute base address of the game state API.
        /// </summary>
        /// <remarks>
        /// This should be an absolute URI, for example: <c>https://api.example.com/</c>.
        /// It is used as the <see cref="HttpClient.BaseAddress"/> for the game state client.
        /// </remarks>
        public string BaseAddress { get; set; } = string.Empty;
    }
}
