using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public interface IDisasterContributionLookup
    {
        DisasterContribution GetDisasterContribution(int disasterCardId);
    }
}
