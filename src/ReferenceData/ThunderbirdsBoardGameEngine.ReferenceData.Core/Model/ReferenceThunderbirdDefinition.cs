using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Core.Model
{
    public sealed record ReferenceThunderbirdDefinition
    {
        public ThunderbirdCode Code { get; }

        public string DisplayName { get; }

        public MovementDomain Domain { get; }

        public int TopSpeed { get; }

        public int CharacterCapacity { get; }

        [JsonConstructor]
        internal ReferenceThunderbirdDefinition(ThunderbirdCode code, string displayName, MovementDomain domain, int topSpeed, int characterCapacity)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(displayName);
            ArgumentOutOfRangeException.ThrowIfNegative(topSpeed);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(characterCapacity);

            Code = code;
            DisplayName = displayName;
            Domain = domain;
            TopSpeed = topSpeed;
            CharacterCapacity = characterCapacity;
        }
    }
}