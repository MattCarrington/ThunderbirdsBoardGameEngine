using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure.GameStateIntegrity
{
    internal class CharacterExistenceLookup : ICharacterExistenceLookup
    {
        private readonly ICharacterDefinitionCatalog _characterDefinitionCatalog;

        public CharacterExistenceLookup(ICharacterDefinitionCatalog characterDefinitionCatalog)
        {
            _characterDefinitionCatalog = characterDefinitionCatalog;
        }

        public bool Exists(CharacterCode characterCode)
        {
            return _characterDefinitionCatalog.Exists(characterCode);
        }
    }
}
