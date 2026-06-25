using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers
{
    public sealed class MovementLocationOptionsMapper
    {
        private readonly ILocationDefinitionCatalog _catalog;

        public MovementLocationOptionsMapper(ILocationDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyList<MovementLocationOptions> ToViewModel(IEnumerable<string> locationCodes)
        {
            var options = new List<MovementLocationOptions>();

            foreach (var code in locationCodes)
            {
                if (!_catalog.TryGetByCode(new LocationCode(code), out var location))
                {
                    continue;
                }

                options.Add(new MovementLocationOptions(location.Code.Value, location.DisplayName));
            }

            return options;
        }
    }
}
