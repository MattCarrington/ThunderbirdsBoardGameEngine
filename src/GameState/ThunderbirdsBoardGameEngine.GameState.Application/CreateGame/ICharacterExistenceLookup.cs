using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public interface ICharacterExistenceLookup
    {
        bool Exists(CharacterCode characterCode);
    }
}
