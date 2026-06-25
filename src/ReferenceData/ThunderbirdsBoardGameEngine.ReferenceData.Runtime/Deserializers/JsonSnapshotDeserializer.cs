using System.Text.Json;
using ThunderbirdsBoardGameEngine.ReferenceData.Core;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Deserializers
{
    internal class JsonSnapshotDeserializer : ISnapshotDeserializer
    {
        public ReferenceDataSnapshot Deserialize(Stream stream)
        {
            return JsonSerializer.Deserialize<ReferenceDataSnapshot>(stream, SnapshotJsonOptions.Create())
                ?? throw new InvalidOperationException("Failed to deserialize snapshot.");
        }
    }
}