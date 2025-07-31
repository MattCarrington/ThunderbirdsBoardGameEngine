using ThunderbirdsBoardGameEngine.GameData.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Entities
{
    public abstract class BonusCondition
    {
        public int BonusValue { get; set; }

        public BoardLocation? Location { get; set; }
    }
}
