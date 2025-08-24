using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public sealed class ThunderbirdBonusCondition : BonusCondition
    {
        public ThunderbirdMachine Thunderbird { get; set; }
    }
}
