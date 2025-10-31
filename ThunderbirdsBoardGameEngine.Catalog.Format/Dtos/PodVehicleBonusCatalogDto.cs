using System.Text.Json.Serialization;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    public sealed record PodVehicleBonusCatalogDto : BonusConditionCatalogDto
    {
        [JsonPropertyOrder(2)]
        public required string PodVehicle { get; init; } = default!;
    }
}
