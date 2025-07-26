using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public abstract class Bonus
    {
        public int BonusValue { get; set; }

        public BoardLocation? Location { get; set; }
    }
}
