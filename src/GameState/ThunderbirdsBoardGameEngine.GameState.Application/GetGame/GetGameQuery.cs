using MediatR;

namespace ThunderbirdsBoardGameEngine.GameState.Application.GetGame
{
    public sealed record GetGameQuery(Guid GameId) : IRequest<GetGameResponse>;
}
