using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.DisasterBonus;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.Factories
{
    [Obsolete("Catalog API is deprecated. Use Reference Data instead")]
    public static class DisasterBonusKeyFactory
    {
        public static DisasterBonusKey ForCharacter(Character name)
        {
            return Create("character", name);
        }

        public static DisasterBonusKey ForPodVehicle(PodVehicle name)
        {
            return Create("podvehicle", name);
        }

        public static DisasterBonusKey ForThunderbird(ThunderbirdMachine name)
        {
            return Create("thunderbird", name);
        }

        private static DisasterBonusKey Create(string category, Enum value)
        {
            var normalised = value
                .ToString()
                .ToLowerInvariant();

            return new DisasterBonusKey($"{category}:{normalised}");
        }
    }
}