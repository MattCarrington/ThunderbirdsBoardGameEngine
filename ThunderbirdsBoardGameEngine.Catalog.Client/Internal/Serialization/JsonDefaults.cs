using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.Catalog.Client.Internal.Serialization
{
    internal static class JsonDefaults
    {
        public static readonly JsonSerializerOptions CamelCase = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            PropertyNameCaseInsensitive = true
        };
    }
}
