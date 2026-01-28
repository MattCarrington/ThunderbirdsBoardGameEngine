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
            var disaster = _disasterContributionLookup.GetDisasterContribution(query.DisasterCardCode);

            var input = new RescueCalculationInput(query.PresentDisasterBonusKeys);

            var sources = new List<IBonusModifierSource> { disaster };

            var calculatedTarget = _rescueTargetCalculator.CalculateRescueTarget(disaster.DifficultyNumber, input, sources);

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
