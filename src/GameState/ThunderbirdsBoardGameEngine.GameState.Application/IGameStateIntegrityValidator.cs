using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.Application
{
    public interface IGameStateIntegrityValidator
    {
        void Validate(Game game);
    }
}
