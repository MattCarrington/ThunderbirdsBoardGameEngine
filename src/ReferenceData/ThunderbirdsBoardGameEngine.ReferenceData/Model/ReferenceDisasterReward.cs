using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "rewardType")]
    [JsonDerivedType(typeof(SpecificToken), "specificToken")]
    [JsonDerivedType(typeof(PlayerChoice), "playerChoice")]
    public abstract record ReferenceDisasterReward
    {
        private ReferenceDisasterReward() { }

        public sealed record SpecificToken(BonusToken Token) : ReferenceDisasterReward;

        public sealed record PlayerChoice : ReferenceDisasterReward;
    }
}
