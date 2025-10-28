using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Converters;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Stubs;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Factories
{
    public static class JsonOptionsFactory
    {
        public static JsonOptionsMonitor CreateJsonOptions()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                PropertyNameCaseInsensitive = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.Converters.Add(new BonusConverter()); // TEMP

            return new JsonOptionsMonitor(options); 
        }
    }
}
