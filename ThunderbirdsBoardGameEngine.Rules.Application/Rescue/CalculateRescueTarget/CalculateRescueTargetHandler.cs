namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.CalculateRescueTarget
{
    public class CalculateRescueTargetHandler
    {
        private readonly IRescueContextProvider _rescueContextProvider;
        private readonly BonusCalculator _bonusCalculator;

        public CalculateRescueTargetHandler(IRescueContextProvider rescueContextProvider, BonusCalculator bonusCalculator)
        {
            _rescueContextProvider = rescueContextProvider;
            _bonusCalculator = bonusCalculator;
        }

        public CalculateRescueTargetResponse Handle(CalculateRescueTargetRequest request)
        {
            var context = _rescueContextProvider.GetRescueContext(request.DisasterCardId);

            var calculatedTarget = _bonusCalculator.CalculateRescueTarget(request.AppliedBonusKeys, context);

            return new CalculateRescueTargetResponse
            (
                TargetNumber: calculatedTarget.TargetRoll,
                TotalBonus: calculatedTarget.TotalBonus,
                AppliedBonuses: calculatedTarget.AppliedBonuses                    
            );
        }
    }
}
