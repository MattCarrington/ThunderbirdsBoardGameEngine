namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed class MovementValidator
    {
        public MovementValidationResult Validate(MovementRequest request)
        {
            if (!request.Topography.Locations.Any(c => c.Key == request.Start))
            {
                return new MovementValidationResult(false, request.Start.Value, "Unknown start location");
            }

            if (!request.Topography.Locations.Any(c => c.Key == request.Destination))
            {
                return new MovementValidationResult(false, request.Destination.Value, "Unknown destination location");
            }

            if (request.Thunderbird.TopSpeed <= 0)
            {
                return new MovementValidationResult(false, request.Thunderbird.Key.Value, $"Invalid movement range: {request.Thunderbird.TopSpeed}");
            }

            return new MovementValidationResult(true);
        }
    }

    public sealed record MovementValidationResult(
        bool IsValid,
        string? ErrorCode = null,
        string? ErrorMessage = null);
}
