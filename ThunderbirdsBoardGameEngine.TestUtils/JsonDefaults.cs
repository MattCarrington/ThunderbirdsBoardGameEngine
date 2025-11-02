using System.Text.Json;
using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.TestUtils
{
    public static class JsonDefaults
    {
        public static JsonSerializerOptions DisasterCards { get; } = CreateDisasterCardsOptions();

        private static JsonSerializerOptions CreateDisasterCardsOptions()
        {
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            };

            options.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;

            options.Converters.Add(new JsonStringEnumConverter());

            return options;
        }
    }
}
