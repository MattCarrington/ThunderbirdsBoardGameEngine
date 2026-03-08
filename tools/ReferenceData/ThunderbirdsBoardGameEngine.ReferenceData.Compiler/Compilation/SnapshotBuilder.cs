using ThunderbirdsBoardGameEngine.Catalog.Generator.Helpers;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Enums;
using ThunderbirdsBoardGameEngine.PublishedLanguage.Identities;
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

            return new ReferenceDataSnapshot(
                SchemaVersion: 1,
                ContentVersion: "1.0.0",
                DisasterDefinitions: disasters,
                CharacterDefinitions: new List<ReferenceCharacterDefinition>(),
                ThunderbirdDefinitions: new List<ReferenceThunderbirdDefinition>(),
                PodVehicleDefinitions: new List<ReferencePodVehicleDefinition>(),
                LocationDefinitions: new List<ReferenceLocationDefinition>()
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
    }
}
