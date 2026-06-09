using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public class ValidateMovementService : IValidateMovementService
    {
        private readonly IMovementClient _client;
        private readonly MovementResultMapper _mapper;

        public ValidateMovementService(IMovementClient client, MovementResultMapper mapper)
        {
            _client = client;
            _mapper = mapper;
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

            return result.Success ? _mapper.ToViewModel(result.Data!) : null;
        }
    }
}
