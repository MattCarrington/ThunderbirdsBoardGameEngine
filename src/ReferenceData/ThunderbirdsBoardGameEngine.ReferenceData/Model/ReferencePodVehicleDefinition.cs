using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferencePodVehicleDefinition
    {
        public PodVehicleCode Code { get; }

        public string DisplayName { get; }

        public ReferencePodVehicleDefinition(PodVehicleCode code, string displayName)
        {
            ArgumentNullException.ThrowIfNullOrWhiteSpace(displayName);

            Code = code;
            DisplayName = displayName;
        }
    }
}
