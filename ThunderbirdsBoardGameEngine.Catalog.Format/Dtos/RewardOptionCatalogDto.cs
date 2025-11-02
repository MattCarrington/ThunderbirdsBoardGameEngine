using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.Catalog.Format.Discriminators;

namespace ThunderbirdsBoardGameEngine.Catalog.Format.Dtos
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "type")]
    [JsonDerivedType(typeof(TokenRewardCatalogDto), RewardTokenDiscriminator.Token)]
    [JsonDerivedType(typeof(PlayerChoiceRewardCatalogDto), RewardTokenDiscriminator.PlayerChoice)]
    public abstract record RewardOptionCatalogDto;
}
