using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs;
using ThunderbirdsBoardGameEngine.Serialization.Converters;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Factories
{
    public static class JsonOptionsFactory
    {
        public static JsonOptionsSnapshot CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.Converters.Add(new BonusConverter()); // TEMP

            return new JsonOptionsSnapshot(options);
        }
    }
}
