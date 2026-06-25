using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
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
