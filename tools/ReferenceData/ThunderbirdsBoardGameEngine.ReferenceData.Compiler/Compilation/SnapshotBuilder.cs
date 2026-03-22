using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source;
using ThunderbirdsBoardGameEngine.ReferenceData.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotBuilder
    {
        public ReferenceDataSnapshot Build(CompilationContext context)
        {
            var disasters = context.Disasters.Select(d =>
                new ReferenceDisasterDefinition(
                    new CardCode(StringHelpers.Slugify(d.Name)),
                    StringHelpers.NormalizeWhitespace(d.Name, nameof(d.Name)),
                    d.DifficultyNumber,
                    new LocationCode(StringHelpers.Slugify(d.Location)),
                    Enum.Parse<RescueType>(d.RescueType, true),
                    MapBonuses(d.Bonuses),
                    MapRewards(d.Rewards)
                )
            ).ToList();

            var locations = context.Locations.Select(l =>
                new ReferenceLocationDefinition(
                    new LocationCode(StringHelpers.Slugify(l.Name)),
                    StringHelpers.NormalizeWhitespace(l.Name, nameof(l.Name))
                )
            ).ToList();

            var characterDefinitions = BuildCharacterDefinitions(context.Characters);

            var thunderbirdDefinitions = BuildThunderbirdDefinitions(context.Thunderbirds);

            var podVehicleDefinitions = BuildPodVehicleDefinitions(context.PodVehicles);

            return new ReferenceDataSnapshot(
                SchemaVersion: 1,
                ContentVersion: "1.0.0",
                DisasterDefinitions: disasters,
                LocationDefinitions: locations,
                CharacterDefinitions: characterDefinitions,
                ThunderbirdDefinitions: thunderbirdDefinitions,
                PodVehicleDefinitions: podVehicleDefinitions
            );
        }

        private static IReadOnlyList<ReferenceDisasterBonus> MapBonuses(IEnumerable<BonusInput> bonuses)
        {
            return bonuses.Select(b =>
                new ReferenceDisasterBonus(
                    new DisasterBonusKey(StringHelpers.Slugify(b.TargetName)),
                    b.Value,
                    b.Location is null ? null : new LocationCode(StringHelpers.Slugify(b.Location))
                )
            ).ToList();
        }

        private static IReadOnlyList<ReferenceDisasterReward> MapRewards(IEnumerable<string> rewards)
        {
            return rewards.Select(r =>
                r.Equals("User Choice", StringComparison.OrdinalIgnoreCase)
                    ? (ReferenceDisasterReward)new ReferenceDisasterReward.PlayerChoice()
                    : new ReferenceDisasterReward.SpecificToken(
                        Enum.Parse<BonusToken>(r, true))
            ).ToList();
        }

        private static List<ReferenceCharacterDefinition> BuildCharacterDefinitions(
            List<CharacterInput> characterInputs)
        {
            return characterInputs
                .Select(input =>
                {
                    var code = new CharacterCode(StringHelpers.Slugify(input.Name));

                    // Build optional rescue bonus (null for Lady Penelope)
                    ReferenceCharacterRescueBonus? rescueBonus = null;
                    if (!string.IsNullOrWhiteSpace(input.RescueType) && input.BonusValue.HasValue)
                    {
                        var rescueType = Enum.Parse<RescueType>(input.RescueType, ignoreCase: true);
                        rescueBonus = new ReferenceCharacterRescueBonus(rescueType, input.BonusValue.Value);
                    }

                    return new ReferenceCharacterDefinition(
                        code,
                        input.Name,
                        rescueBonus);
                })
                .ToList();
        }

        private static List<ReferenceThunderbirdDefinition> BuildThunderbirdDefinitions(
            List<ThunderbirdInput> thunderbirdInputs)
        {
            return thunderbirdInputs
                .Select(input => new ReferenceThunderbirdDefinition(
                    new ThunderbirdCode(StringHelpers.Slugify(input.Name)),
                    StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name))))
                .ToList();
        }

        private static List<ReferencePodVehicleDefinition> BuildPodVehicleDefinitions(
            List<PodVehicleInput> podVehicleInputs)
        {
            return podVehicleInputs
                .Select(input => new ReferencePodVehicleDefinition(
                    new PodVehicleCode(StringHelpers.Slugify(input.Name)),
                    StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name))))
                .ToList();
        }
    }
}
