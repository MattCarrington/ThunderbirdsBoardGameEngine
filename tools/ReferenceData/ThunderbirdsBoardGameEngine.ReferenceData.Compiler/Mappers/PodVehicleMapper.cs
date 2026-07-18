using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Helpers;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Inputs;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Model;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Mappers
{
    public sealed class PodVehicleMapper
    {
        public IEnumerable<ReferencePodVehicleDefinition> Map(IEnumerable<PodVehicleInput> inputs)
        {
            return inputs.Select(MapPodVehicle);
        }
        private ReferencePodVehicleDefinition MapPodVehicle(PodVehicleInput input)
        {
            return new ReferencePodVehicleDefinition(
                code: new PodVehicleCode(StringHelpers.Slugify(input.Name)),
                displayName: StringHelpers.NormalizeWhitespace(input.Name, nameof(input.Name)));
        }
    }
}
