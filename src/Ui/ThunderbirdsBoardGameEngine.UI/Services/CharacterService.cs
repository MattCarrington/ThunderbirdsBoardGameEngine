using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Mappers;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Services
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
            return _catalog.GetAll().Select(c => c.ToViewModel()).ToList();
        }
    }
}
