using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Domain.Exceptions
{
    public sealed class ThunderbirdMachineNotFoundException : Exception
    {
        private ThunderbirdMachineNotFoundException(string message)
            : base(message)
        {
        }

        public static ThunderbirdMachineNotFoundException Create(ThunderbirdCode thunderbird)
        {
            return new ThunderbirdMachineNotFoundException($"Thunderbird machine '{thunderbird.Value}' not found.");
        }
    }
}
