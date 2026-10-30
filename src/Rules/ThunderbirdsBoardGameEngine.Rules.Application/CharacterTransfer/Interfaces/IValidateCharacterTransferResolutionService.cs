using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.CharacterTransfer.Interfaces
{
    public interface IValidateCharacterTransferResolutionService
    {
        CharacterTransferEvaluationResult ResolveTransferValidation(ValidateCharacterTransferInput input);
    }
}