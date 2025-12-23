using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.Rescue;

namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetHandler : IRequestHandler<CalculateRescueTargetQuery, CalculateRescueTargetResponse>
    {
        private readonly IDisasterContributionLookup _disasterContributionLookup;
        private readonly RescueTargetCalculator _rescueTargetCalculator;

        public CalculateRescueTargetHandler(IDisasterContributionLookup disasterContributionLookup, RescueTargetCalculator rescueTargetCalculator)
        {
            _disasterContributionLookup = disasterContributionLookup;
            _rescueTargetCalculator = rescueTargetCalculator;
        }

        public Task<CalculateRescueTargetResponse> Handle(CalculateRescueTargetQuery query, CancellationToken cancellationToken)
        {
            var projection = _disasterContributionLookup.GetDisasterContribution(query.DisasterCardId);

            var calculatedTarget = _rescueTargetCalculator.CalculateRescueTarget(query.AppliedBonusKeys, projection);

            return Task.FromResult(
                new CalculateRescueTargetResponse
                (
                    TargetNumber: calculatedTarget.TargetRoll,
                    TotalBonus: calculatedTarget.TotalBonus,
                    AppliedBonuses: calculatedTarget.AppliedBonuses
                )
            );
        }
    }
}
