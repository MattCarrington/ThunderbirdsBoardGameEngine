using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Core;

namespace ThunderbirdsBoardGameEngine.ReferenceData.ComponentTests.Helpers
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
