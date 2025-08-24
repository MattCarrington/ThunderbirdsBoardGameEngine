using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public sealed class CharacterBonusCondition : BonusCondition
    {
        public Character Character { get; set; }
    }
}
