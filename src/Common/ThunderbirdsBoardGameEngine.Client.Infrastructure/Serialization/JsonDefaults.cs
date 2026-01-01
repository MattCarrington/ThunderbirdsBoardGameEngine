using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.Client.Infrastructure.Serialization
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
