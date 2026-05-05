using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Output
{
    public static class JsonSnapshotWriter
    {
        public static void Write(ReferenceDataSnapshot snapshot)
        {
            var json = JsonSerializer.Serialize(snapshot, SnapshotJsonOptions.Default);

            File.WriteAllText("snapshot.json", json);
        }
    }
}
