namespace ThunderbirdsBoardGameEngine.GameData.Api.Messages.Dtos
{
    public class DisasterCardDto
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public int DifficultyNumber { get; set; }

        public string Location { get; set; }

        public string RescueType { get; set; }

        public IReadOnlyList<BonusDto> Bonuses { get; set; }

        public IReadOnlyList<RewardDto> Rewards { get; set; }
    }
}
