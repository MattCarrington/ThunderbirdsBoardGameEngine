using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Deserializers
{
    public class JsonSnapshotDeserializer : ISnapshotDeserializer
    {
        public ReferenceDataSnapshot Deserialize(Stream stream)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) }
            };

            return JsonSerializer.Deserialize<ReferenceDataSnapshot>(stream, options)
                ?? throw new InvalidOperationException("Failed to deserialize snapshot.");
        }
    }
}