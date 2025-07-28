using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public sealed class ThunderbirdBonusCondition : BonusCondition
    {
        public ThunderbirdMachine Thunderbird { get; set; }
    }
}
