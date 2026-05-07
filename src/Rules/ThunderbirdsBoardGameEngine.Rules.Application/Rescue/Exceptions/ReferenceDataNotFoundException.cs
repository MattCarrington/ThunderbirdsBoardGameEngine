namespace ThunderbirdsBoardGameEngine.Rules.Application.Rescue.Exceptions
{
    public sealed class ReferenceDataNotFoundException : Exception
    {
        public string ResourceType { get; }

        public string Code { get; }

        public ReferenceDataNotFoundException(string resourceType, string code)
            : base($"{resourceType} '{code}' was not found in reference data.")
        {
            ResourceType = resourceType;
            Code = code;
        }
    }
}
