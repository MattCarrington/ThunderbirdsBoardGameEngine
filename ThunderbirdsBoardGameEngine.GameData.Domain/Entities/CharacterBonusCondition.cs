using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Entities
{
    public sealed class CharacterBonusCondition : BonusCondition
    {
        public Character Character { get; set; }
    }
}
