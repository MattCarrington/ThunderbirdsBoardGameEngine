using System.Text.Json;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests
{
    public static class JsonTestSerializer
    {
        public static string Serialize<T>(T value)
        {
            return JsonSerializer.Serialize(value, SnapshotJsonOptions.Create());
        }

        public static T? Deserialize<T>(string json)
        {
            return JsonSerializer.Deserialize<T>(json, SnapshotJsonOptions.Create());
        }
    }
}
