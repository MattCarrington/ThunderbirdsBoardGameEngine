using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceEventCardDefinition
    {
        public CardCode Code { get; }

        public string DisplayName { get; }

        [JsonConstructor]
        internal ReferenceEventCardDefinition(
            CardCode code,
            string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}
