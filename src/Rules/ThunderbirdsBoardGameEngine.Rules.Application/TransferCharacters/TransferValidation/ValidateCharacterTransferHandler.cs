using MediatR;
using ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.Interfaces;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters;
using ThunderbirdsBoardGameEngine.Rules.Domain.TransferCharacters.Evaluation;

namespace ThunderbirdsBoardGameEngine.Rules.Application.TransferCharacters.TransferValidation
{
    public class ValidateCharacterTransferHandler : IRequestHandler<ValidateCharacterTransferQuery, ValidateCharacterTransferResponse>
    {
        private readonly IValidateCharacterTransferResolutionService _validationService;

        public ValidateCharacterTransferHandler(IValidateCharacterTransferResolutionService validationService)
        {
            _validationService = validationService;
        }

        public Task<ValidateCharacterTransferResponse> Handle(ValidateCharacterTransferQuery request, CancellationToken cancellationToken)
        {
            var input = new ValidateCharacterTransferInput(
                Thunderbirds: request.CurrentState,
                Assignments: request.ProposedAssignments);

            var result = _validationService.ResolveTransferValidation(input);

            return Task.FromResult(result.ToResponse());
        }
    }
}
