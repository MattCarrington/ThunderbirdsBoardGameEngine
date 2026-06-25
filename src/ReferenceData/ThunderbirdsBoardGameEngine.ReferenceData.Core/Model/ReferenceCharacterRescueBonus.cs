using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
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
