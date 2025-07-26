using ThunderbirdsBoardGameEngine.GameData.Api.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.GameData.Api.Domain.Entities
{
    public sealed class CharacterBonus : Bonus
    {
        public Character Character { get; set; }
    }
}
