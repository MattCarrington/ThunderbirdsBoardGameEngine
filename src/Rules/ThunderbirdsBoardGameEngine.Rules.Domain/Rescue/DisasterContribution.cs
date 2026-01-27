using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public sealed record DisasterContribution(int DifficultyNumber, IReadOnlyList<DisasterBonus> AvailableBonuses, RescueType RescueType);
}