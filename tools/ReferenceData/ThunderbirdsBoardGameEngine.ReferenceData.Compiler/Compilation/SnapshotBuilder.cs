using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers;
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

            var characterMapper = new CharacterMapper();

            var thunderbirdMapper = new ThunderbirdMapper();

            var characterDefinitions = characterMapper.Map(context.Characters).ToList();

            var thunderbirdDefinitions = thunderbirdMapper.Map(context.Thunderbirds).ToList();

            var podVehicleDefinitions = BuildPodVehicleDefinitions(context.PodVehicles);

            var mapEdgeDefinitions = BuildMapEdgeDefinitions(context.MapEdges, locationCodeResolver);

            var fabCardDefinitions = BuildFabCardDefinitions(context.FabCards);

            var eventCardDefinitions = BuildEventCardDefinitions(context.EventCards);

            var disasterBonusTargetResolver = new DisasterBonusTargetResolver(characterDefinitions, podVehicleDefinitions, thunderbirdDefinitions);

            var disasterCardMapper = new DisasterCardMapper(locationCodeResolver, disasterBonusTargetResolver);

            return new ReferenceDataSnapshot(
                SchemaVersion: SnapshotVersions.SchemaVersion,
                ContentVersion: SnapshotVersions.ContentVersion,
                GeneratedAt: _clock.UtcNow,
                GeneratorVersion: SnapshotVersions.GeneratorVersion,
                DisasterDefinitions: disasterCardMapper.Map(context.Disasters).ToList(),
                LocationDefinitions: locations,
                CharacterDefinitions: characterDefinitions,
                ThunderbirdDefinitions: thunderbirdDefinitions,
                PodVehicleDefinitions: podVehicleDefinitions,
                MapEdgeDefinitions: mapEdgeDefinitions,
                FabCardDefinitions: fabCardDefinitions,
                EventCardDefinitions: eventCardDefinitions
            );
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
