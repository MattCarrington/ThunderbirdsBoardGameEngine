using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Writers
{
    public class JsonSnapshotWriter : ISnapshotWriter
    {
        public void Write(ReferenceDataSnapshot snapshot)
        {
            var json = JsonSerializer.Serialize(snapshot, SnapshotJsonOptions.Create());

            File.WriteAllText("snapshot.json", json);
        }
    }
}
