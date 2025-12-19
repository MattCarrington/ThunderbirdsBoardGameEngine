using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces
{
    public interface IDisasterContributionLookup
    {
        DisasterContribution GetDisasterContribution(int disasterCardId);
    }
}
