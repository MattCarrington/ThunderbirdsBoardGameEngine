using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Serialization.Converters;

namespace ThunderbirdsBoardGameEngine.TestUtils
{
    public static class JsonDefaults
    {
        public static JsonSerializerOptions DisasterCards { get; } = CreateDisasterCardsOptions();

        private static JsonSerializerOptions CreateDisasterCardsOptions()
        {
            var options = new JsonSerializerOptions 
            { 
                PropertyNameCaseInsensitive = true 
            };

            options.Converters.Add(new JsonStringEnumConverter());
            options.Converters.Add(new BonusConverter());

            return options;
        }
    }
}
