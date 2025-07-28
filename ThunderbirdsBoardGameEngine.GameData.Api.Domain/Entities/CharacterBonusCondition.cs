using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public sealed class CharacterBonusCondition : BonusCondition
    {
        public Character Character { get; set; }
    }
}
