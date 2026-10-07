using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers
{
    public sealed class MovementLocationOptionsMapper
    {
        private readonly ILocationDefinitionCatalog _catalog;

        public MovementLocationOptionsMapper(ILocationDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyList<LocationOptionsViewModel> ToViewModel(IEnumerable<string> locationCodes)
        {
            var options = new List<LocationOptionsViewModel>();

            foreach (var code in locationCodes)
            {
                if (!_catalog.TryGetByCode(new LocationCode(code), out var location))
                {
                    continue;
                }

                options.Add(new LocationOptionsViewModel(location.Code.Value, location.DisplayName));
            }

            return options;
        }
    }
}
