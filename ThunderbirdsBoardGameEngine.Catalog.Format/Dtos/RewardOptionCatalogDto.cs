using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Schema;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(TokenRewardCatalogDto), RewardTokenDiscriminator.Token)]
    [JsonDerivedType(typeof(PlayerChoiceRewardCatalogDto), RewardTokenDiscriminator.PlayerChoice)]
    public abstract record RewardOptionCatalogDto;
}
