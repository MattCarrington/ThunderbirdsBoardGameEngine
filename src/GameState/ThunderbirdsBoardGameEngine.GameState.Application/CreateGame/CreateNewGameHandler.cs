using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public sealed class CreateNewGameHandler : IRequestHandler<CreateNewGameCommand, CreateNewGameResult>
    {
        private readonly StandardGameSetupFactory _gameFactory;
        private readonly IGameStateIntegrityValidator _gameStateIntegrityValidator;
        private readonly IGameRepository _gameRepository;

        public CreateNewGameHandler(StandardGameSetupFactory standardGameSetupFactory, IGameStateIntegrityValidator gameStateIntegrityValidator, IGameRepository gameRepository)
        {
            _gameFactory = standardGameSetupFactory;
            _gameStateIntegrityValidator = gameStateIntegrityValidator;
            _gameRepository = gameRepository;
        }

        public async Task<CreateNewGameResult> Handle(CreateNewGameCommand request, CancellationToken cancellationToken)
        {
            var game = _gameFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);

            _gameStateIntegrityValidator.Validate(game);

            await _gameRepository.SaveGameSession(game, cancellationToken);

            return new CreateNewGameResult(game);
        }
    }
}
