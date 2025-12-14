using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    public sealed class ThunderbirdBonusCondition : BonusCondition
    {
        public ThunderbirdMachine Thunderbird { get; }

        public ThunderbirdBonusCondition(ThunderbirdMachine thunderbird, int value) : this(thunderbird, value, null)
        {
        }

        public ThunderbirdBonusCondition(ThunderbirdMachine thunderbird, int value, BoardLocation? location) : base(value, location)
        {
            Thunderbird = thunderbird;
        }
    }
}
