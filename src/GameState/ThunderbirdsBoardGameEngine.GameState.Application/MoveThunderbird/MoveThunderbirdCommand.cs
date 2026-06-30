using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.MoveThunderbird
{
    public record MoveThunderbirdCommand(Guid GameId, ThunderbirdCode ThunderbirdCode, LocationCode Destination) : IRequest<MoveThunderbirdResponse>;
}
