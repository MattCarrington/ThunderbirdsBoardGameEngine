using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Deserializers
{
    internal sealed class DisasterCardDeserializer : IDisasterCardDeserializer
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public DisasterCardDeserializer(IOptionsMonitor<JsonSerializerOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions.Get(CatalogJson.Name) ?? throw new ArgumentNullException(nameof(jsonOptions));
        }

        public IReadOnlyList<DisasterCardCatalogDto> Deserialize(JsonElement data)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<DisasterCardCatalogDto>>(data.GetRawText(), _jsonOptions)
                ?? [];
        }
    }
}
