using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Deserializers
{
    internal class CharacterDefinitionDeserializer : ICharacterDefinitionDeserializer
    {
        private readonly JsonSerializerOptions _jsonOptions;

        public CharacterDefinitionDeserializer(IOptionsMonitor<JsonSerializerOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions.Get(CatalogJson.Name) ?? throw new ArgumentNullException(nameof(jsonOptions));
        }

        public IReadOnlyList<CharacterCatalogDto> Deserialize(JsonElement data)
        {
            return JsonSerializer.Deserialize<IReadOnlyList<CharacterCatalogDto>>(data.GetRawText(), _jsonOptions)
                ?? [];
        }
    }
}
