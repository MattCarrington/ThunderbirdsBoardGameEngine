using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public sealed record CharacterBonusCatalogDto : BonusConditionCatalogDto
    {
        [JsonPropertyOrder(2)]
        public required string Character { get; init; } = default!;
    }
}
