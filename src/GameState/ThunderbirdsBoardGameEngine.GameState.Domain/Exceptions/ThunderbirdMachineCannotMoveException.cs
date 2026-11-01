using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions
{
    public sealed class ThunderbirdMachineCannotMoveException : Exception
    {
        private ThunderbirdMachineCannotMoveException(string message)
            : base(message)
        {
        }

        public static ThunderbirdMachineCannotMoveException NoCharactersAboard(ThunderbirdCode thunderbird)
        {
            return new ThunderbirdMachineCannotMoveException($"Thunderbird machine '{thunderbird.Value}' cannot move because there are no characters aboard.");
        }
    }
}
