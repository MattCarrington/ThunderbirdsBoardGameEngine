using ThunderbirdsBoardGameEngine.ReferenceData.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Model
{
    public sealed record ReferenceThunderbirdDefinition
    {
        public ThunderbirdCode Code { get; }

        public string DisplayName { get; }

        public MovementDomain Domain { get; }

        public int TopSpeed { get; }

        public ReferenceThunderbirdDefinition(ThunderbirdCode code, string displayName, MovementDomain domain, int topSpeed)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
            ArgumentOutOfRangeException.ThrowIfNegative(topSpeed);

            Code = code;
            DisplayName = displayName;
            Domain = domain;
            TopSpeed = topSpeed;
        }
    }
}