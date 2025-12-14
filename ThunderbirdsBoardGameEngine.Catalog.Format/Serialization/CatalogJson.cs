using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Serialization
{
    public static class CatalogJson
    {
        public const string Name = "Catalog";

        public static readonly JsonSerializerOptions Catalog = Create();

        // For DI scenarios, call Configure(options)
        public static void Configure(JsonSerializerOptions options)
        {
            var src = Catalog;
            options.PropertyNamingPolicy = src.PropertyNamingPolicy;
            options.PropertyNameCaseInsensitive = src.PropertyNameCaseInsensitive;
            options.DefaultIgnoreCondition = src.DefaultIgnoreCondition;
            options.WriteIndented = src.WriteIndented;

            // Copy converters (avoid duplicating instances)
            options.Converters.Clear();

            foreach (var converter in src.Converters)
            {
                options.Converters.Add(converter);
            }
        }

        private static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                WriteIndented = true
            };

            return options;
        }
    }
}
