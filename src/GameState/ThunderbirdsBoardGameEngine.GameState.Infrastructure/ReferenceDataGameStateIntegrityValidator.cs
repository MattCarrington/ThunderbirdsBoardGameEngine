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
            ValidateThunderbirdMachines(game.Machines);
            ValidateCharacters(game.Characters);
        }

        private void ValidateCharacters(IReadOnlyDictionary<CharacterCode, ThunderbirdCode> characterState)
        {
            if (characterState.Count != _characterCatalog.GetAll().Count())
            {
                throw new InvalidOperationException("Character count does not match reference data.");
            }

            foreach (var character in characterState)
            {
                if (_characterCatalog.GetByCode(character.Key) == null) //  TODO: Consider adding TryGetByCode instead of GetByCode to avoid potential exceptions
                {
                    throw new InvalidOperationException($"Character {character.Key} does not exist in reference data.");
                }
                if (!_thunderbirdCatalog.TryGetByCode(character.Value, out _))
                {
                    throw new InvalidOperationException($"Thunderbird {character.Value} does not exist in reference data.");
                }
            }
        }

        private void ValidateThunderbirdMachines(IReadOnlyDictionary<ThunderbirdCode, LocationCode> thunderbirdState)
        {
            if (thunderbirdState.Count != _thunderbirdCatalog.GetAll().Count())
            {
                throw new InvalidOperationException("Thunderbird count does not match reference data.");
            }

            foreach (var thunderbird in thunderbirdState)
            {
                if (!_thunderbirdCatalog.TryGetByCode(thunderbird.Key, out _))
                {
                    throw new InvalidOperationException($"Thunderbird {thunderbird.Key} does not exist in reference data.");
                }

                if (!_locationCatalog.Exists(thunderbird.Value))
                {
                    throw new InvalidOperationException($"Location {thunderbird.Value} does not exist in reference data.");
                }
            }
        }
    }
}
