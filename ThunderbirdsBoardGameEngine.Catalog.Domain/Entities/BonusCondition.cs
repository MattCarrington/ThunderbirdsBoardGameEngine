using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{

    public abstract class BonusCondition
    {
        public int BonusValue { get; }

        public BoardLocation? Location { get; }

        protected BonusCondition(int value, BoardLocation? location)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(value);

            BonusValue = value;
            Location = location;
        }
    }
}
