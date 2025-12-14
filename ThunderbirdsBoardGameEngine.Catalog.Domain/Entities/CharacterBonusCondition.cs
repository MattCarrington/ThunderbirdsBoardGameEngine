using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public sealed class CharacterBonusCondition : BonusCondition
    {
        public Character Character { get; }

        public CharacterBonusCondition(Character character, int value) : this(character, value, null)
        {
        }

        public CharacterBonusCondition(Character character, int value, BoardLocation? location) : base(value, location)
        {
            Character = character;
        }
    }
}
