using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceDisasterBonus
    {
        public DisasterBonusKey Key { get; }

        public int Value { get; }

        public LocationCode? Location { get; }

        public ReferenceDisasterBonus(
            DisasterBonusKey key,
            int value,
            LocationCode? location)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            Key = key;
            Value = value;
            Location = location;
        }
    }
}
