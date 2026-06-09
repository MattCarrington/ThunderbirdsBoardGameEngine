using ThunderbirdsBoardGameEngine.Rules.Client.Interfaces.V1;
using ThunderbirdsBoardGameEngine.Rules.Contracts.Dtos.Movement.ValidateMovement.V1;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public class ValidateMovementService : IValidateMovementService
    {
        private readonly IMovementClient _client;

        public ValidateMovementService(IMovementClient client)
        {
            _client = client;
        }

        public async Task<ValidateMovementResponseDto?> ValidateMovementAsync(
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

            return result.Success ? result.Data : null;
        }
    }
}
