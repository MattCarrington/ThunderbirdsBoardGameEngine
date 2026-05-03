using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceThunderbirdDefinition
    {
        public ThunderbirdCode Code { get; }

        public string DisplayName { get; }

        public ReferenceThunderbirdDefinition(ThunderbirdCode code, string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}