using ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters;
using ThunderbirdsBoardGameEngine.GameState.Domain;

namespace ThunderbirdsBoardGameEngine.GameState.ComponentTests.TransferCharacters
{
    public class FakeCharacterTransferRulesGateway : ICharacterTransferRulesGateway
    {
        private readonly CharacterTransferDecision _decision;

        public FakeCharacterTransferRulesGateway(CharacterTransferDecision decision)
        {
            _decision = decision;
        }

        public Task<CharacterTransferDecision> ValidateTransferCharactersAsync(
            IReadOnlyCollection<CurrentThunderbirdState> currentThunderbirdState,
            IReadOnlyCollection<CharacterAssignment> characterAssignments,
            CancellationToken cancellationToken)
        {
            return Task.FromResult(_decision);
        }
    }
}
