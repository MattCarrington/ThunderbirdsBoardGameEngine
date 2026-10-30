using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public class ValidateCharacterTransferResolutionService : IValidateCharacterTransferResolutionService
    {
        private readonly IThunderbirdTransferDefinitionLookup _lookup;
        private readonly TransferCharacterEvaluator _evaluator;

        public ValidateCharacterTransferResolutionService(IThunderbirdTransferDefinitionLookup lookup, TransferCharacterEvaluator evaluator)
        {
            _lookup = lookup ?? throw new ArgumentNullException(nameof(lookup));
            _evaluator = evaluator ?? throw new ArgumentNullException(nameof(evaluator));
        }

        public TransferCharacterEvaluationResult ResolveTransferValidation(ValidateCharacterTransferInput input)
        {
            ArgumentNullException.ThrowIfNull(input, nameof(input));

            var thunderbirds = input.Thunderbirds
                .Select(thunderbird => new ThunderbirdTransferState(
                    ThunderbirdCode: thunderbird.ThunderbirdCode,
                    CharacterCapacity: _lookup.GetThunderbirdCharacterCapacity(thunderbird.ThunderbirdCode),
                    CurrentLocation: thunderbird.CurrentLocation,
                    Occupants: thunderbird.Occupants))
                .ToArray();

            var evaluationInput = new TransferCharacterEvaluationInput(
                thunderbirds,
                input.Assignments);

            return _evaluator.Evaluate(evaluationInput);
        }
    }
}
