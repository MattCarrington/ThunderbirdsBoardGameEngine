using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Entities
{
    public sealed class ThunderbirdBonusCondition : BonusCondition
    {
        public ThunderbirdMachine Thunderbird { get; set; }
    }
}
