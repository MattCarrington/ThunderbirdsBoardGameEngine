using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.GameData.Domain.Entities;
using ThunderbirdsBoardGameEngine.Serialization.Converters;

namespace ThunderbirdsBoardGameEngine.GameData.Importer.Output
{
    public static class JsonWriter
    {
        public static JsonSerializerOptions CreateDefaultOptions()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };

            options.Converters.Add(new JsonStringEnumConverter(JsonNamingPolicy.CamelCase));
            options.Converters.Add(new BonusConverter());

            return options;
        }

        public static void WriteJson(IEnumerable<DisasterCard> cards, string outputPath, JsonSerializerOptions options)
        {
            var json = JsonSerializer.Serialize(cards, options);
            Directory.CreateDirectory(Path.GetDirectoryName(outputPath)!);
            File.WriteAllText(outputPath, json);
        }
    }
}
