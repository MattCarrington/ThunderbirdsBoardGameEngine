using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterDefinitionCatalog _catalog;

        public CharacterService(ICharacterDefinitionCatalog catalog)
        {
            _catalog = catalog;
        }

        public IReadOnlyList<CharacterViewModel> GetAll()
        {
            return _catalog.GetAll().Select(c => new CharacterViewModel(c.Code.Value, c.DisplayName)).ToList();
        }
    }
}
