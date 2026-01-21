using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public class CharacterDefinition
    {
        public Character Key { get; }

        public CharacterRescueBonus? RescueBonus { get; }

        public CharacterDefinition(Character key, CharacterRescueBonus? characterRescueBonus)
        {
            if (key == Character.LadyPenelope && characterRescueBonus is not null)
            {
                throw new ArgumentException("Lady Penelope cannot have a rescue bonus.");
            }

            Key = key;
            RescueBonus = characterRescueBonus;
        }

        public CharacterDefinition(Character key) : this(key, null)
        {
        }
    }
}
