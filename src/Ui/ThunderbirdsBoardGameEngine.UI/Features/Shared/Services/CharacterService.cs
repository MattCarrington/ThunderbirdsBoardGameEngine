using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Shared.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterDefinitionCatalog _catalog;

        public CharacterService(ICharacterDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyList<SelectionOptionViewModel> GetAll()
        {
            return _catalog.GetAll()
                .Select(c => new SelectionOptionViewModel(
                    c.Code.Value,
                    c.DisplayName))
                .ToList();
        }
    }
}
