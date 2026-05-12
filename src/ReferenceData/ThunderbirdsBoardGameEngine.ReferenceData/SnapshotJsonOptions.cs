using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;

namespace ThunderbirdsBoardGameEngine.ReferenceData
{
    /// <summary>
    /// Provides pre-configured JSON serialization options for reference data snapshots.
    /// </summary>
    /// <remarks>
    /// These options ensure consistent JSON formatting across reference data serialization and deserialization,
    /// including custom converters for strongly-typed identity codes (e.g., <see cref="CardCode"/>, <see cref="LocationCode"/>).
    /// <para>
    /// The JSON format uses:
    /// <list type="bullet">
    /// <item>camelCase property naming</item>
    /// <item>Indented formatting for readability</item>
    /// <item>Null value omission</item>
    /// <item>Relaxed JSON escaping for special characters</item>
    /// </list>
    /// </para>
    /// </remarks>
    public static class SnapshotJsonOptions
    {
        /// <summary>
        /// Gets a shared, read-only instance of configured JSON options.
        /// </summary>
        /// <remarks>
        /// Use this property when you don't need to modify the options. 
        /// For scenarios requiring option modifications, use <see cref="Create"/> instead.
        /// </remarks>
        public static JsonSerializerOptions Default { get; } = Create();

        /// <summary>
        /// Creates a new instance of configured JSON serialization options for reference data.
        /// </summary>
        /// <returns>
        /// A <see cref="JsonSerializerOptions"/> instance configured with custom converters 
        /// and formatting settings for reference data snapshots.
        /// </returns>
        /// <remarks>
        /// Use this method when you need to customize the options further or require a separate instance.
        /// The returned options include custom converters for all strongly-typed identity codes used in reference data.
        /// </remarks>
        public static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.Converters.Add(new CardCodeJsonConverter());
            options.Converters.Add(new DisasterBonusKeyJsonConverter());
            options.Converters.Add(new PodVehicleCodeJsonConverter());
            options.Converters.Add(new ThunderbirdCodeJsonConverter());
            options.Converters.Add(new CharacterCodeJsonConverter());
            options.Converters.Add(new LocationCodeJsonConverter());

            return options;
        }
    }
}
