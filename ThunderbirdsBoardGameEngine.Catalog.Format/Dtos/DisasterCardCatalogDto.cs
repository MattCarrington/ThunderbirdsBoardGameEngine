using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public sealed record DisasterCardCatalogDto
    {
        [JsonPropertyOrder(0)]
        public required int Id { get; init; }

        [JsonPropertyOrder(1)]
        public required string Name { get; init; } = default!;

        [JsonPropertyOrder(2)]
        public required string RescueType { get; init; } = default!;

        [JsonPropertyOrder(3)]
        public required int DifficultyNumber { get; init; }

        [JsonPropertyOrder(4)]
        public required string Location { get; init; } = default!;

        [JsonPropertyOrder(5)]
        public required List<BonusConditionCatalogDto> BonusConditions { get; init; }

        [JsonPropertyOrder(6)]
        public required List<RewardOptionCatalogDto> RewardOptions { get; init; }

        [JsonPropertyOrder(7)]
        public required string Code { get; init; } = default!;
    }
}
