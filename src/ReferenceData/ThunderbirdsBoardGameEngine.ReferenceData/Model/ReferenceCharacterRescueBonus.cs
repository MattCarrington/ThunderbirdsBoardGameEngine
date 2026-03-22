using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceCharacterRescueBonus
    {
        public RescueType RescueType { get; }

        public int Value { get; }

        public ReferenceCharacterRescueBonus(RescueType rescueType, int value)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            RescueType = rescueType;
            Value = value;
        }
    }
}
