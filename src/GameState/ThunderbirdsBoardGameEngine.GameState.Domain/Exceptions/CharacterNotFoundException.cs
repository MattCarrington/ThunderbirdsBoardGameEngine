using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions
{
    public sealed class CharacterNotFoundException : Exception
    {
        private CharacterNotFoundException(string message)
            : base(message)
        {
        }

        public static CharacterNotFoundException Create(CharacterCode character)
        {
            return new CharacterNotFoundException($"Character '{character.Value}' not found.");
        }
    }
}
