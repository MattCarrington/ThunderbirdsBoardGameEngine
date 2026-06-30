using MediatR;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public record CreateGameSessionCommand() : IRequest<CreateGameSessionResponse>;
}
