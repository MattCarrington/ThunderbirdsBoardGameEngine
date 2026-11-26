using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions
{
    public sealed class ThunderbirdMachineCannotMoveException : Exception
    {
        private ThunderbirdMachineCannotMoveException(string message)
            : base(message)
        {
        }

        public static ThunderbirdMachineCannotMoveException ActiveCharacterNotAboard(ThunderbirdCode machineCode, CharacterCode activeCharacter)
        {
            return new ThunderbirdMachineCannotMoveException(
                $"Thunderbird machine '{machineCode.Value}' cannot move because the active character '{activeCharacter.Value}' is not aboard.");
        }
    }
}
