using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public sealed record ThunderbirdBonusCatalogDto : BonusConditionCatalogDto
    {
        [JsonPropertyOrder(2)]
        public required string Thunderbird { get; init; } = default!;
    }
}
