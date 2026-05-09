using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Mappers;
using ThunderbirdsBoardGameEngine.UI.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Services
{
    public class CharacterService : ICharacterService
    {
        private readonly ICharacterDefinitionCatalog _catalog;
        private readonly CharacterMapper _mapper;

        public CharacterService(ICharacterDefinitionCatalog catalog, CharacterMapper mapper)
        {
            _catalog = catalog;
            _mapper = mapper;
        }

        public IReadOnlyList<CharacterViewModel> GetAll()
        {
            return _catalog.GetAll().Select(c => _mapper.ToViewModel(c)).ToList();
        }
    }
}
