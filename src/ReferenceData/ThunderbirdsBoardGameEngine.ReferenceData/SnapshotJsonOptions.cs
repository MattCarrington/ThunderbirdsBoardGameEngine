using System.Text.Json;
using System.Text.Json.Serialization;
using ThunderbirdsBoardGameEngine.ReferenceData.Serialization;

namespace ThunderbirdsBoardGameEngine.ReferenceData
{
    public static class SnapshotJsonOptions
    {
        public static JsonSerializerOptions Default { get; } = Create();

        public static JsonSerializerOptions Create()
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                WriteIndented = true,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull
            };

            options.Converters.Add(new CardCodeJsonConverter());
            options.Converters.Add(new DisasterBonusKeyJsonConverter());
            options.Converters.Add(new PodVehicleCodeJsonConverter());
            options.Converters.Add(new ThunderbirdCodeJsonConverter());
            options.Converters.Add(new CharacterCodeJsonConverter());
            options.Converters.Add(new LocationCodeJsonConverter());

            return options;
        }
    }
}
