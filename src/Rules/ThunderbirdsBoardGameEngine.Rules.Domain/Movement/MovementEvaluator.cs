namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed class MovementEvaluator
    {
        private readonly BreadthFirstRouteFinder _routeFinder;
        private readonly ActionPointCalculator _actionPointCalculator;

        public MovementEvaluator(BreadthFirstRouteFinder routeFinder, ActionPointCalculator actionPointCalculator)
        {
            _routeFinder = routeFinder ?? throw new ArgumentNullException(nameof(routeFinder));
            _actionPointCalculator = actionPointCalculator ?? throw new ArgumentNullException(nameof(actionPointCalculator));
        }

        public MovementEvaluationResult Evaluate(MovementInput input)
        {
            if (input.Thunderbird.TopSpeed <= 0)
            {
                return MovementEvaluationResult.InvalidMove(
                    $"{input.Thunderbird.Key.Value} cannot move.");
            }

            var route = _routeFinder.FindShortestRoute(input);

            if (route is null)
            {
                return MovementEvaluationResult.InvalidMove(
                    $"No route found from {input.Start.Value} to {input.Destination.Value} for {input.Thunderbird.Key.Value}.");
            }

            var actionPointCost = _actionPointCalculator.CalculateActionPoints(
                route.SpacesTravelled,
                input.Thunderbird.TopSpeed);

            return MovementEvaluationResult.ValidMove(
                route.Route,
                route.SpacesTravelled,
                input.Thunderbird.TopSpeed,
                actionPointCost);
        }
    }
}
