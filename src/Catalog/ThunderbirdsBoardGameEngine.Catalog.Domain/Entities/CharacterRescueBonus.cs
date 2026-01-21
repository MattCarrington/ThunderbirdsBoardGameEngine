using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public class CharacterRescueBonus
    {
        public RescueType RescueType { get; }

        public int BonusValue { get; }

        public CharacterRescueBonus(RescueType rescueType, int bonusValue)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(bonusValue);

            RescueType = rescueType;
            BonusValue = bonusValue;
        }
    }
}
