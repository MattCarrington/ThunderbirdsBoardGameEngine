using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Routing;
using ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Speed;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement.Evaluation
{
    public sealed class MovementEvaluator
    {
        private readonly IRouteFinder _routeFinder;
        private readonly IMovementSpeedModifierSourceRegistry _registry;
        private readonly ActionPointCalculator _actionPointCalculator;

        public MovementEvaluator(IRouteFinder routeFinder, IMovementSpeedModifierSourceRegistry registry, ActionPointCalculator actionPointCalculator)
        {
            _routeFinder = routeFinder ?? throw new ArgumentNullException(nameof(routeFinder));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
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

            AppliedMovementSpeedModifier? applicableModifier = null;

            foreach (var eventCard in input.EventCards)
            {
                if (!_registry.TryGetEventCard(eventCard, out var source))
                {
                    continue;
                }

                applicableModifier = source.ApplyMovementModifier(input.Thunderbird.Key);

                if (applicableModifier is not null)
                {
                    break;
                }
            }

            var effectiveTopSpeed = applicableModifier?.EffectiveTopSpeed ?? input.Thunderbird.TopSpeed;

            var actionPointCost = _actionPointCalculator.CalculateActionPoints(
                route.SpacesTravelled,
                effectiveTopSpeed);

            return MovementEvaluationResult.ValidMove(
                route: route.Route,
                spacesTravelled: route.SpacesTravelled,
                topSpeed: effectiveTopSpeed,
                actionPointCost: actionPointCost,
                messages: applicableModifier is null
                    ? []
                    : [applicableModifier.Message]);
        }
    }
}
