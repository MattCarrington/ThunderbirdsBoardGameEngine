using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.TestUtils.Helpers
{
    public static class TestDataLoader
    {
        private static readonly JsonSerializerOptions _jsonOptions = new() { PropertyNameCaseInsensitive = true };

        public static T LoadJsonFromFile<T>(string filename, string folder = "ExpectedData")
        {
            var basePath = Path.Combine(AppContext.BaseDirectory, folder);

            if (!Directory.Exists(basePath))
            {
                // fallback or debug info
                throw new DirectoryNotFoundException($"Test data directory not found at {basePath}");
            }

            var fullPath = Path.Combine(basePath, filename);

            if (!File.Exists(fullPath))
                throw new FileNotFoundException($"Expected test data not found: {fullPath}");

            var json = File.ReadAllText(fullPath);
            return JsonSerializer.Deserialize<T>(json, _jsonOptions) ?? throw new InvalidOperationException("Failed to deserialize expected disaster card DTOs.");
        }
    }
}
