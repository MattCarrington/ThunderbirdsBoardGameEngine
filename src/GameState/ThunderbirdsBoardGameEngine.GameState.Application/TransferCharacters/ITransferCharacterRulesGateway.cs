using ThunderbirdsBoardGameEngine.GameState.Domain;


namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    public interface ITransferCharacterRulesGateway
    {
        Task<CharacterTransferDecision> ValidateTransferCharactersAsync(IReadOnlyCollection<CurrentThunderbirdState> currentThunderbirdState, IReadOnlyCollection<CharacterAssignment> characterAssignments, CancellationToken cancellationToken);
    }
}
