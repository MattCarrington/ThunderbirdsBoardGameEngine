using ThunderbirdsBoardGameEngine.GameState.Application;
using ThunderbirdsBoardGameEngine.GameState.Domain;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;

namespace ThunderbirdsBoardGameEngine.GameState.Infrastructure
{
    internal class ReferenceDataGameStateIntegrityValidator : IGameStateIntegrityValidator
    {
        private readonly ILocationDefinitionCatalog _locationCatalog;
        private readonly IThunderbirdDefinitionCatalog _thunderbirdCatalog;
        private readonly ICharacterDefinitionCatalog _characterCatalog;

        public ReferenceDataGameStateIntegrityValidator(
            ILocationDefinitionCatalog locationCatalog,
            IThunderbirdDefinitionCatalog thunderbirdCatalog,
            ICharacterDefinitionCatalog characterCatalog)
        {
            _locationCatalog = locationCatalog;
            _thunderbirdCatalog = thunderbirdCatalog;
            _characterCatalog = characterCatalog;
        }

        public void Validate(Game game)
        {
            ArgumentNullException.ThrowIfNull(game, nameof(game));

            if (game.Id == Guid.Empty)
            {
                throw new InvalidOperationException("Game ID cannot be empty.");
            }

            ValidateThunderbirdMachines(game.Machines);
            ValidateCharacters(game.Characters);
        }

        private void ValidateCharacters(IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characterState)
        {
            var expectedCharacterCodes = _characterCatalog
                .GetAll()
                .Select(character => character.Code)
                .ToHashSet();

            if (!expectedCharacterCodes.SetEquals(characterState.Keys))
            {
                throw new InvalidOperationException(
                    "Game characters do not match reference data.");
            }

            foreach (var character in characterState)
            {
                if (!_thunderbirdCatalog.TryGetByCode(character.Value, out _))
                {
                    throw new InvalidOperationException($"Thunderbird {character.Value} does not exist in reference data.");
                }
            }
        }

        private void ValidateThunderbirdMachines(IReadOnlyDictionary<ThunderbirdCode, LocationCode> thunderbirdState)
        {
            var expectedThunderbirdCodes = _thunderbirdCatalog
                .GetAll()
                .Select(thunderbird => thunderbird.Code)
                .ToHashSet();

            if (!expectedThunderbirdCodes.SetEquals(thunderbirdState.Keys))
            {
                throw new InvalidOperationException(
                    "Game thunderbirds do not match reference data.");
            }

            foreach (var thunderbird in thunderbirdState)
            {
                if (!_locationCatalog.Exists(thunderbird.Value))
                {
                    throw new InvalidOperationException($"Location {thunderbird.Value} does not exist in reference data.");
                }
            }
        }
    }
}
