using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public abstract record ReferenceDisasterReward
    {
        private ReferenceDisasterReward() { }

        public sealed record SpecificToken(BonusToken Token) : ReferenceDisasterReward;

        public sealed record PlayerChoice : ReferenceDisasterReward;
    }
}
