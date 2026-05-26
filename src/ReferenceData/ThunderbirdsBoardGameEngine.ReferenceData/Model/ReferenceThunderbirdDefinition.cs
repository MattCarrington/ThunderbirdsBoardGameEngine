using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceThunderbirdDefinition
    {
        public ThunderbirdCode Code { get; }

        public string DisplayName { get; }
        public TraversalDomain Domain { get; }

        public ReferenceThunderbirdDefinition(ThunderbirdCode code, string displayName, TraversalDomain domain)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
            Domain = domain;
        }
    }
}