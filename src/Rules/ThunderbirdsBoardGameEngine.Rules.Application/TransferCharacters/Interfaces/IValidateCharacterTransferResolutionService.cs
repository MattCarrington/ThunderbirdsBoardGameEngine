using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.Interfaces
{
    public interface IValidateCharacterTransferResolutionService
    {
        TransferCharacterEvaluationResult ResolveTransferValidation(ValidateCharacterTransferInput input);
    }
}