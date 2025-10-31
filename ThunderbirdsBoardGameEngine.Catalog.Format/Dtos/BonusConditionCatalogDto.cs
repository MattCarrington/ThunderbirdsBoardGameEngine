using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Schema;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(CharacterBonusCatalogDto), BonusConditionDiscriminators.CharacterBonus)]
    [JsonDerivedType(typeof(ThunderbirdBonusCatalogDto), BonusConditionDiscriminators.ThunderbirdBonus)]
    [JsonDerivedType(typeof(PodVehicleBonusCatalogDto), BonusConditionDiscriminators.PodVehicleBonus)]
    public abstract record BonusConditionCatalogDto
    {
        [JsonPropertyOrder(0)]
        public required int BonusValue { get; init; }

        [JsonPropertyOrder(1)]
        public string? Location { get; init; } // optional condition-location code
    }
}
