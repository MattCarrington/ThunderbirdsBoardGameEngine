namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions
{
    public sealed class InvalidRescueCalculationRequestException : Exception
    {
        public string ResourceType { get; }

        public string Code { get; }

        public InvalidRescueCalculationRequestException(string resourceType, string code)
            : base($"{resourceType} '{code}' was not found in reference data.")
        {
            ResourceType = resourceType;
            Code = code;
        }
    }
}
