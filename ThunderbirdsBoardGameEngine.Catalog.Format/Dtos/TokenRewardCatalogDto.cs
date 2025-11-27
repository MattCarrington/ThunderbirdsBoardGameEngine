using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public sealed record TokenRewardCatalogDto : RewardOptionCatalogDto
    {
        [JsonPropertyOrder(0)]
        public required string Token { get; init; } = default!;
    }
}
