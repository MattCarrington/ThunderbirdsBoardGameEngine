using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public sealed record MoveThunderbirdCommand(Guid GameId, ThunderbirdCode Thunderbird, LocationCode Destination) : IRequest<MoveThunderbirdResult>;
}
