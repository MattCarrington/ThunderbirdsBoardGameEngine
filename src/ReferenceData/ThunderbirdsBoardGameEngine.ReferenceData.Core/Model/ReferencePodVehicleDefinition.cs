using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferencePodVehicleDefinition
    {
        public PodVehicleCode Code { get; }

        public string DisplayName { get; }

        public ReferencePodVehicleDefinition(PodVehicleCode code, string displayName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}
