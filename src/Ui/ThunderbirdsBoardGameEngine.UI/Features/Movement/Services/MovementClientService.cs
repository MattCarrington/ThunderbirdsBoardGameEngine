using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Mappers;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Services
{
    public class MovementClientService : IMovementClientService
    {
        private readonly IMovementClient _client;
        private readonly MovementResultMapper _resultMapper;
        private readonly MovementLocationOptionsMapper _locationsMapper;

        public MovementClientService(IMovementClient client, MovementResultMapper resultMapper, MovementLocationOptionsMapper locationsMapper)
        {
            _client = client;
            _resultMapper = resultMapper;
            _locationsMapper = locationsMapper;
        }

        public async Task<MovementResultViewModel?> ValidateMovementAsync(
            string thunderbirdCode,
            string startLocationCode,
            string destinationLocationCode)
        {
            var request = new ValidateMovementRequestDto
            {
                StartLocation = startLocationCode,
                DestinationLocation = destinationLocationCode
            };

            var result = await _client.ValidateMovementAsync(thunderbirdCode, request);

            return result.Success ? _resultMapper.ToViewModel(result.Data!) : null;
        }

        public async Task<IReadOnlyList<MovementLocationOptions>> GetAccessibleLocationsAsync(string thunderbirdCode)
        {
            var result = await _client.GetAccessibleLocationsAsync(thunderbirdCode);

            return result.Success
                ? _locationsMapper.ToViewModel(result.Data!.AccessibleLocations)
                : Array.Empty<MovementLocationOptions>();
        }
    }
}
