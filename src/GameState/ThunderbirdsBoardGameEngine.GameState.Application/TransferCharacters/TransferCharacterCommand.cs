using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Domain;


namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    public sealed record TransferCharacterCommand(Guid GameId, IReadOnlyCollection<CharacterAssignment> CharacterAssignments) : IRequest<TransferCharacterResult>;
}
