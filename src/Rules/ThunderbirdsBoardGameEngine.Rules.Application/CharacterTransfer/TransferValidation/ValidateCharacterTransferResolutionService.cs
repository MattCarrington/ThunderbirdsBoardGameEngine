using ThunderbirdsBoardGameEngine.Rules.Application.CharacterTransfer.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.CharacterTransfer.TransferValidation
{
    internal class ValidateCharacterTransferResolutionService
    {
        private readonly IThunderbirdTransferDefinitionLookup _lookup;
        private readonly CharacterTransferEvaluator _evaluator;

        public ValidateCharacterTransferResolutionService(IThunderbirdTransferDefinitionLookup lookup, CharacterTransferEvaluator evaluator)
        {
            _lookup = lookup;
            _evaluator = evaluator;
        }

        public CharacterTransferEvaluationResult ResolveTransferValidation(ValidateCharacterTransferInput input)
        {
            var thunderbirds = input.Thunderbirds
                .Select(thunderbird => new ThunderbirdTransferState(
                    ThunderbirdCode: thunderbird.ThunderbirdCode,
                    CharacterCapacity: _lookup.GetThunderbirdCapacity(thunderbird.ThunderbirdCode),
                    CurrentLocation: thunderbird.CurrentLocation,
                    Occupants: thunderbird.Occupants))
                .ToArray();

            var evaluationInput = new CharacterTransferEvaluationInput(
                thunderbirds,
                input.Assignments);

            return _evaluator.Evaluate(evaluationInput);
        }
    }
}
