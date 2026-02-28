using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output
{
    public static class JsonSnapshotWriter
    {
        public static void Write(ReferenceDataSnapshot snapshot)
        {
            var json = JsonSerializer.Serialize(snapshot, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            });

            File.WriteAllText("snapshot.json", json);
        }
    }
}
