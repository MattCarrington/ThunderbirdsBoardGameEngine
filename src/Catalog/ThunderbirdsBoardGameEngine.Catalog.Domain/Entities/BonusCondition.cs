using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Entities
{
    /// <summary>
    /// Represents a condition under which a bonus modifier is applied.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Bonus conditions may optionally specify a location.
    /// </para>
    /// <para>
    /// If <see cref="Location"/> is <c>null</c>, the condition applies at the
    /// same location as the parent disaster card. This mirrors the behaviour
    /// of the physical game, where bonuses without an explicit location
    /// requirement must be achieved at the disaster’s location.
    /// </para>
    /// </remarks>
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
