using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceLocationDefinition
    {
        public LocationCode Code { get; }

        public string DisplayName { get; }

        public MovementDomain Domain { get; }

        [JsonConstructor]
        internal ReferenceLocationDefinition(LocationCode code, string displayName, MovementDomain domain)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
            Domain = domain;
        }
    }
}
