using MediatR;

namespace ThunderbirdsBoardGameEngine.GameState.Application.GetGame
{
    public record GetGameSessionQuery(Guid GameId) : IRequest<GetGameSessionResponse>;
}
