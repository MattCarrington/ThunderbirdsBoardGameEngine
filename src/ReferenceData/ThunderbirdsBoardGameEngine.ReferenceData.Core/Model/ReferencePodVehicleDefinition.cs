using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferencePodVehicleDefinition
    {
        public PodVehicleCode Code { get; }

        public string DisplayName { get; }

        [JsonConstructor]
        internal ReferencePodVehicleDefinition(PodVehicleCode code, string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}
