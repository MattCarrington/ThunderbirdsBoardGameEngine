using MediatR;
using ThunderbirdsBoardGameEngine.GameState.Application.Exceptions;


namespace ThunderbirdsBoardGameEngine.GameState.Application.TransferCharacters
{
    internal sealed class TransferCharactersHandler : IRequestHandler<TransferCharactersCommand, TransferCharactersResult>
    {
        private readonly IGameRepository _gameRepository;
        private readonly IGameStateIntegrityValidator _gameStateIntegrityValidator;
        private readonly ICharacterTransferRulesGateway _transferCharacterGateway;

        public TransferCharactersHandler(IGameRepository gameRepository, IGameStateIntegrityValidator gameStateIntegrityValidator, ICharacterTransferRulesGateway transferCharacterGateway)
        {
            _gameRepository = gameRepository;
            _gameStateIntegrityValidator = gameStateIntegrityValidator;
            _transferCharacterGateway = transferCharacterGateway;
        }

        public async Task<TransferCharactersResult> Handle(TransferCharactersCommand request, CancellationToken cancellationToken)
        {
            var game = await _gameRepository.GetGameSessionById(request.GameId, cancellationToken)
                ?? throw new GameNotFoundException();

            _gameStateIntegrityValidator.Validate(game);

            if (request.CharacterAssignments.Count == 0)
            {
                throw InvalidCharacterTransferRequestException.NoAssignments();
            }

            if (request.CharacterAssignments.Count > 2)
            {
                throw InvalidCharacterTransferRequestException.TooManyAssignments();
            }

            if (request.CharacterAssignments
                .GroupBy(assignment => assignment.CharacterCode)
                .Any(group => group.Count() > 1))
            {
                throw InvalidCharacterTransferRequestException.DuplicateCharacterAssignment();
            }

            var resolvedAssignments = request.CharacterAssignments
                 .Select(assignment => new
                 {
                     Assignment = assignment,
                     SourceThunderbirdCode = game.GetCharacterAssignment(assignment.CharacterCode)
                 })
                 .ToArray();

            var participatingMachineCodes = resolvedAssignments
                .SelectMany(resolved => new[]
                {
                    resolved.SourceThunderbirdCode,
                    resolved.Assignment.DestinationThunderbirdCode
                })
                .Distinct()
                .ToArray();

            foreach (var resolved in resolvedAssignments)
            {
                game.GetThunderbirdMachineLocation(resolved.Assignment.DestinationThunderbirdCode);
            }

            if (resolvedAssignments.All(resolved =>
                resolved.SourceThunderbirdCode == resolved.Assignment.DestinationThunderbirdCode))
            {
                return new TransferCharactersResult(game);
            }

            if (participatingMachineCodes.Length != 2)
            {
                throw InvalidCharacterTransferRequestException.MustInvolveExactlyTwoMachines();
            }

            var currentState = participatingMachineCodes
                .Select(machineCode => new CurrentThunderbirdState(
                    Thunderbird: machineCode,
                    Location: game.GetThunderbirdMachineLocation(machineCode),
                    CharactersAboard: game.GetThunderbirdMachineOccupancy(machineCode)))
                .ToList();

            var decision = await _transferCharacterGateway.ValidateTransferCharactersAsync(currentState, request.CharacterAssignments, cancellationToken);

            if (!decision.IsPermitted)
            {
                throw CharacterTransferRejectedException.FromDecision(decision);
            }

            game.TransferCharacters(request.CharacterAssignments);

            _gameStateIntegrityValidator.Validate(game);

            await _gameRepository.UpdateGameSession(game, cancellationToken);

            return new TransferCharactersResult(game);
        }
    }
}
