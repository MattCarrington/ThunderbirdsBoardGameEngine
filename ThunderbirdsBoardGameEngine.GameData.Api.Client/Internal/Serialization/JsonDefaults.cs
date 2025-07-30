using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Client.Internal.Serialization
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
