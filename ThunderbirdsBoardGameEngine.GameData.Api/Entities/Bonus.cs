using ThunderbirdsBoardGameEngine.GameData.Api.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Entities
{
    public abstract class Bonus
    {
        public int BonusValue { get; set; }

        public BoardLocation? Location { get; set; }
    }
}
