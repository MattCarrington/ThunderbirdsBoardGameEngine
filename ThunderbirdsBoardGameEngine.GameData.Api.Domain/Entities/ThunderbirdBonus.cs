using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public sealed class ThunderbirdBonus : Bonus
    {
        public ThunderbirdMachine Thunderbird { get; set; }
    }
}
