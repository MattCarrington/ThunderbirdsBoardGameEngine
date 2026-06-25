using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Core;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Writers
{
    public class JsonSnapshotWriter : ISnapshotWriter
    {
        private readonly string _outputPath;

        public JsonSnapshotWriter(string outputPath)
        {
            _outputPath = outputPath;
        }

        public void Write(ReferenceDataSnapshot snapshot)
        {
            var json = JsonSerializer.Serialize(snapshot, SnapshotJsonOptions.Create());

            File.WriteAllText(_outputPath, json);
        }
    }
}
