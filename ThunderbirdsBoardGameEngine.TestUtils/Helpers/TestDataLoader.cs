using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    public static class TestDataLoader
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static async Task<T> LoadJsonFromFileAsync<T>(string fileName)
        {
            var json = await File.ReadAllTextAsync(fileName);

            return JsonSerializer.Deserialize<T>(json, _jsonOptions)
                   ?? throw new InvalidOperationException("Failed to deserialize JSON file: " + fileName);
        }
    }
}
