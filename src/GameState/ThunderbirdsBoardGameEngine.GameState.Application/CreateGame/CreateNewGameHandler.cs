using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Domain.Setup.V1;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public sealed class CreateNewGameHandler : IRequestHandler<CreateNewGameCommand, CreateNewGameResult>
    {
        private readonly StandardGameSetupFactory _gameFactory;
        private readonly IGameRepository _gameRepository;

        public CreateNewGameHandler(StandardGameSetupFactory standardGameSetupFactory, IGameRepository gameRepository)
        {
            _gameFactory = standardGameSetupFactory;
            _gameRepository = gameRepository;
        }

        public async Task<CreateNewGameResult> Handle(CreateNewGameCommand request, CancellationToken cancellationToken)
        {
            var game = _gameFactory.Create(Guid.NewGuid(), DateTimeOffset.UtcNow);  // TODO: Replace "TODO" with the actual setup version

            await _gameRepository.SaveGameSession(game, cancellationToken);

            return new CreateNewGameResult(game);
        }
    }
}
