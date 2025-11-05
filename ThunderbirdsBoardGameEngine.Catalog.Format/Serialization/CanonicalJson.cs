using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Serialization
{
    public static class CanonicalJson
    {
        public static readonly JsonSerializerOptions Options = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            WriteIndented = false            
        };

        public static JsonSerializerOptions Pretty()
        {
            // Clone from canonical to ensure policy stays aligned; only change indentation.
            var options = new JsonSerializerOptions(Options) 
            { 
                WriteIndented = true 
            };

            return options;
        }
    }
}
