using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.Api.Mappers.GameState.V1
{
    public static class GameStateMappingExtensions
    {
        public static GameStateResponseDto ToDto(this Game game)
        {
            var charactersByMachine = game.Characters
                .GroupBy(assignment => assignment.Value)
                .ToDictionary(
                group => group.Key,
                group => (IReadOnlyList<string>)group
                    .Select(assignment => assignment.Key.Value)
                    .OrderBy(code => code)
                    .ToList());

            var machines = game.Machines
                .OrderBy(machine => machine.Key.Value)
                .Select(machine => new ThunderbirdMachineStateDto
                {
                    ThunderbirdCode = machine.Key.Value,
                    LocationCode = machine.Value.Value,
                    OccupantCharacterCodes =
                        charactersByMachine.GetValueOrDefault(machine.Key, [])
                })
                .ToList();

            return new GameStateResponseDto
            {
                GameId = game.Id,
                ThunderbirdMachines = machines
            };
        }
    }
}
