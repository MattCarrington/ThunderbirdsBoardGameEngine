using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation
{
    public sealed class SnapshotBuilder
    {
        private readonly IClock _clock;

        public SnapshotBuilder(IClock clock)
        {
            _clock = clock;
        }

        public ReferenceDataSnapshot Build(CompilationContext context)
        {
            var locations = context.Locations.Select(l =>
                new ReferenceLocationDefinition(
                    new LocationCode(StringHelpers.Slugify(l.Name)),
                    StringHelpers.NormalizeWhitespace(l.Name, nameof(l.Name)),
                    Enum.Parse<MovementDomain>(l.Domain, true)
                )
            ).ToList();

            var locationCodeResolver = new LocationCodeResolver(locations);

            var disasters = context.Disasters.Select(d =>
                new ReferenceDisasterDefinition(
                    new CardCode(StringHelpers.Slugify(d.Name)),
                    StringHelpers.NormalizeWhitespace(d.Name, nameof(d.Name)),
                    d.DifficultyNumber,
                    locationCodeResolver.Resolve(d.Location),
                    Enum.Parse<RescueType>(d.RescueType, true),
                    MapBonuses(d.Bonuses, locationCodeResolver),
                    MapRewards(d.Rewards)
                )
            ).ToList();

            var characterDefinitions = BuildCharacterDefinitions(context.Characters);

            var thunderbirdDefinitions = BuildThunderbirdDefinitions(context.Thunderbirds);

            var podVehicleDefinitions = BuildPodVehicleDefinitions(context.PodVehicles);

            var mapEdgeDefinitions = BuildMapEdgeDefinitions(context.MapEdges, locationCodeResolver);

            var fabCardDefinitions = BuildFabCardDefinitions(context.FabCards);

            var eventCardDefinitions = BuildEventCardDefinitions(context.EventCards);

            return new ReferenceDataSnapshot(
                SchemaVersion: SnapshotVersions.SchemaVersion,
                ContentVersion: SnapshotVersions.ContentVersion,
                GeneratedAt: _clock.UtcNow,
                GeneratorVersion: SnapshotVersions.GeneratorVersion,
                DisasterDefinitions: disasters,
                LocationDefinitions: locations,
                CharacterDefinitions: characterDefinitions,
                ThunderbirdDefinitions: thunderbirdDefinitions,
                PodVehicleDefinitions: podVehicleDefinitions,
                MapEdgeDefinitions: mapEdgeDefinitions,
                FabCardDefinitions: fabCardDefinitions,
                EventCardDefinitions: eventCardDefinitions
            );
        }

        private static IReadOnlyList<ReferenceDisasterBonus> MapBonuses(IEnumerable<BonusInput> bonuses, LocationCodeResolver locationCodeResolver)
        {
            return bonuses.Select(b =>
                new ReferenceDisasterBonus(
                    new DisasterBonusKey(StringHelpers.Slugify(b.TargetName)),
                    b.Value,
                    b.Location is null ? null : locationCodeResolver.Resolve(b.Location)
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
                    StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)),
                    domain: Enum.Parse<MovementDomain>(input.MovementDomain, ignoreCase: true),
                    topSpeed: input.TopSpeed
                ))
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

        private static List<ReferenceMapEdgeDefinition> BuildMapEdgeDefinitions(List<MapEdgeInput> mapEdges, LocationCodeResolver locationCodeResolver)
        {
            return mapEdges.Select(input => new ReferenceMapEdgeDefinition(
                    locationCodeResolver.Resolve(input.Edge1),
                    locationCodeResolver.Resolve(input.Edge2),
                    Enum.Parse<MovementDomain>(input.Domain, ignoreCase: true)))
                .ToList();
        }

        private static List<ReferenceFabCardDefinition> BuildFabCardDefinitions(
            List<FabCardInput> fabCardInputs)
        {
            return fabCardInputs
                .Select(input => new ReferenceFabCardDefinition(
                    new CardCode(StringHelpers.Slugify(input.Name)),
                    StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name))))
                .ToList();
        }

        private static List<ReferenceEventCardDefinition> BuildEventCardDefinitions(
            List<EventCardInput> eventCardInputs)
        {
            return eventCardInputs
                .Select(input => new ReferenceEventCardDefinition(
                    new CardCode(StringHelpers.Slugify(input.Name)),
                    StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name))))
                .ToList();
        }
    }
}
