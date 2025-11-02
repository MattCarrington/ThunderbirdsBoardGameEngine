using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Serialization
{
    internal static class CatalogJson
    {
        public const string Name = "DisasterCards";

        public static void Configure(JsonSerializerOptions options)
        {
            options.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            options.PropertyNameCaseInsensitive = true;
            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
        }
    }
}