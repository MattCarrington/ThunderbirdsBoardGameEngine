using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Resolvers;
using ThunderbirdsBoardGameEngine.ReferenceData.Core;
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
            var locationMapper = new LocationMapper();
            var locations = locationMapper.Map(context.Locations).ToList();
            var locationCodeResolver = new LocationCodeResolver(locations);

            var characterMapper = new CharacterMapper();
            var thunderbirdMapper = new ThunderbirdMapper();
            var podVehicleMapper = new PodVehicleMapper();

            var characterDefinitions = characterMapper.Map(context.Characters).ToList();
            var thunderbirdDefinitions = thunderbirdMapper.Map(context.Thunderbirds).ToList();
            var podVehicleDefinitions = podVehicleMapper.Map(context.PodVehicles).ToList();

            var disasterBonusTargetResolver = new DisasterBonusTargetResolver(characterDefinitions, podVehicleDefinitions, thunderbirdDefinitions);
            var disasterCardMapper = new DisasterCardMapper(locationCodeResolver, disasterBonusTargetResolver);

            var mapEdgeMapper = new EdgeMapper(locationCodeResolver);

            var fabCardMapper = new FabCardMapper();
            var eventCardMapper = new EventCardMapper();

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
                MapEdgeDefinitions: mapEdgeMapper.Map(context.MapEdges).ToList(),
                FabCardDefinitions: fabCardMapper.Map(context.FabCards).ToList(),
                EventCardDefinitions: eventCardMapper.Map(context.EventCards).ToList()
            );
        }
    }
}
