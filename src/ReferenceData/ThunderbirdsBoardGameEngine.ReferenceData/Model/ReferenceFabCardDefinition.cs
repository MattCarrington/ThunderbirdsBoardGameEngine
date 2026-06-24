using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceFabCardDefinition
    {
        public CardCode Code { get; }

        public string DisplayName { get; }

        public ReferenceFabCardDefinition(
            CardCode code,
            string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}
