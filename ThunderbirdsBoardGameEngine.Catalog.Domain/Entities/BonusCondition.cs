using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public abstract class BonusCondition
    {
        public int BonusValue { get; set; }

        public BoardLocation? Location { get; set; }
    }
}
