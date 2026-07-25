using MediatR;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public sealed record CreateNewGameCommand() : IRequest<CreateNewGameResult>;
}
