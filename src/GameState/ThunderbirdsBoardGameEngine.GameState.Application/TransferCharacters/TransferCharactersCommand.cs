using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Domain;


namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    public sealed record TransferCharactersCommand(Guid GameId, IReadOnlyCollection<CharacterAssignment> CharacterAssignments) : IRequest<TransferCharactersResult>;
}
