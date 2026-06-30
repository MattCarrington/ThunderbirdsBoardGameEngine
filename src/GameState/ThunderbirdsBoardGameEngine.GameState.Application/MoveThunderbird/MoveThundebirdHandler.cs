using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.CreateGame;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public class MoveThundebirdHandler : IRequestHandler<MoveThunderbirdCommand, MoveThunderbirdResponse>
    {
        private readonly IGameSessionRespository _gameSessionRespository;
        private readonly IValidateMovementGateway _validateMovementGateway;

        public MoveThundebirdHandler(IGameSessionRespository gameSessionRespository, IValidateMovementGateway validateMovementGateway)
        {
            _gameSessionRespository = gameSessionRespository;
            _validateMovementGateway = validateMovementGateway;
        }

        public async Task<MoveThunderbirdResponse> Handle(MoveThunderbirdCommand request, CancellationToken cancellationToken)
        {
            var gameSession = await _gameSessionRespository.GetGameSession(request.GameId, cancellationToken)
                ?? throw new Exception(nameof(request));    // TODO: Create a more specific exception type

            var startLocation = gameSession.GetVehicleLocation(request.ThunderbirdCode);

            var isValid = await _validateMovementGateway.ValidateMovement(request.ThunderbirdCode, startLocation, request.Destination, cancellationToken);

            if (isValid)
            {
                gameSession.MoveVehicleLocation(request.ThunderbirdCode, request.Destination);
                await _gameSessionRespository.SaveGameSession(gameSession, cancellationToken);
            }

            return new MoveThunderbirdResponse(gameSession);
        }
    }
}
