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

        public sealed record SpecificToken : ReferenceDisasterReward
        {
            public BonusToken Token { get; }

            [JsonConstructor]
            internal SpecificToken(BonusToken token)
            {
                Token = token;
            }

            public void Deconstruct(out BonusToken Token) => Token = this.Token;
        }

        public sealed record PlayerChoice : ReferenceDisasterReward
        {
            [JsonConstructor]
            internal PlayerChoice() { }
        }
    }
}
