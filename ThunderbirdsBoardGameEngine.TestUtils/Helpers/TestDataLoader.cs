using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    public static class TestDataLoader
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static T LoadJsonFromFile<T>(string fileName)
        {
            var json = File.ReadAllText(fileName);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions)
                   ?? throw new InvalidOperationException("Failed to deserialize JSON file: " + fileName);
        }
    }
}
