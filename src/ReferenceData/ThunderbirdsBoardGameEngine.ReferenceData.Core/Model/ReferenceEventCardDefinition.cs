using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceEventCardDefinition
    {
        public CardCode Code { get; }

        public string DisplayName { get; }

        public ReferenceEventCardDefinition(
            CardCode code,
            string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}
