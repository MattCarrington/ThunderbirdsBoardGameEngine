using ThunderbirdsBoardGameEngine.GameData.Api.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Entities
{
    public sealed class CharacterBonus : Bonus
    {
        public Character Character { get; set; }
    }
}
