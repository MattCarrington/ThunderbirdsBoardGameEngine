using MediatR;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;

namespace ThunderbirdsBoardGameEngine.GameState.Application.CreateGame
{
    public sealed record CreateNewGameCommand(IReadOnlyList<CharacterCode> TurnOrder) : IRequest<CreateNewGameResult>;
}
