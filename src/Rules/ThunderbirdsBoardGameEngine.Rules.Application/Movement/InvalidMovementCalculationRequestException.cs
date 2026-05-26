namespace ThunderbirdsBoardGameEngine.Rules.Application.Movement
{
    public sealed class InvalidMovementCalculationRequestException : Exception
    {
        public string ResourceType { get; }

        public string Code { get; }

        public InvalidMovementCalculationRequestException(string resourceType, string code)
            : base($"{resourceType} '{code}' was not found in reference data.")
        {
            ResourceType = resourceType;
            Code = code;
        }
    }
}
