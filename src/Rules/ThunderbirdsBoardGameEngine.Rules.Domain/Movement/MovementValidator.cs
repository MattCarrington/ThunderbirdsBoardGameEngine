namespace ThunderbirdsBoardGameEngine.Rules.Domain.Movement
{
    public sealed class MovementValidator
    {
        public MovementValidationResult Validate(MovementInput request)
        {
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
