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

        private readonly Dictionary<string, IReadOnlyList<MovementLocationOptions>> _accessibleLocationsCache = new();

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
            if (_accessibleLocationsCache.TryGetValue(thunderbirdCode, out var cachedLocations))
            {
                return cachedLocations;
            }

            var result = await _client.GetAccessibleLocationsAsync(thunderbirdCode);

            var locations = result.Success
                ? _locationsMapper.ToViewModel(result.Data!.AccessibleLocations)
                : Array.Empty<MovementLocationOptions>();

            _accessibleLocationsCache[thunderbirdCode] = locations;

            return locations;
        }
    }
}
