using ThunderbirdsBoardGameEngine.GameState.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Mappers
{
    public class GameDashboardMapper
    {
        private readonly IThunderbirdDefinitionCatalog _thunderbirdDefinitionCatalog;
        private readonly ILocationDefinitionCatalog _locationDefinitionCatalog;
        private readonly ICharacterDefinitionCatalog _characterDefinitionCatalog;

        public GameDashboardMapper(
            IThunderbirdDefinitionCatalog thunderbirdDefinitionCatalog,
            ILocationDefinitionCatalog locationDefinitionCatalog,
            ICharacterDefinitionCatalog characterDefinitionCatalog)
        {
            _thunderbirdDefinitionCatalog = thunderbirdDefinitionCatalog;
            _locationDefinitionCatalog = locationDefinitionCatalog;
            _characterDefinitionCatalog = characterDefinitionCatalog;
        }

        public GameDashboardViewModel ToViewModel(GameStateResponseDto game)
        {
            return new GameDashboardViewModel(
                GameId: game.GameId,
                ThunderbirdMachines: game.ThunderbirdMachines.Select(MapThunderbirds).ToList(),
                TurnOrder: game.TurnOrder.Select(MapTurnOrder).ToList(),
                ActiveCharacterDisplayName: _characterDefinitionCatalog.GetByCode(new CharacterCode(game.ActiveCharacterCode)).DisplayName,
                ActiveCharacterCode: game.ActiveCharacterCode);
        }

        private ThunderbirdMachinesDashboardViewModel MapThunderbirds(ThunderbirdMachineStateDto dto)
        {
            var thunderbirdDefinition = _thunderbirdDefinitionCatalog.GetByCode(new ThunderbirdCode(dto.ThunderbirdCode));

            return new ThunderbirdMachinesDashboardViewModel(
                ThunderbirdCode: dto.ThunderbirdCode,
                ThunderbirdDisplayName: thunderbirdDefinition.DisplayName,
                LocationCode: dto.LocationCode,
                LocationDisplayName: _locationDefinitionCatalog.GetByCode(new LocationCode(dto.LocationCode)).DisplayName,
                Occupants: dto.OccupantCharacterCodes.Select(MapOccupants).ToList(),
                CharacterCapacity: thunderbirdDefinition.CharacterCapacity);
        }

        private OccupantsDashboardViewModel MapOccupants(string occupant)
        {
            return new OccupantsDashboardViewModel(
                CharacterCode: occupant,
                CharacterDisplayName: _characterDefinitionCatalog.GetByCode(new CharacterCode(occupant)).DisplayName);
        }

        private TurnOrderCharacterDisplayModel MapTurnOrder(string characterCode)
        {
            return new TurnOrderCharacterDisplayModel(
                CharacterDisplayName: _characterDefinitionCatalog.GetByCode(new CharacterCode(characterCode)).DisplayName,
                CharacterCode: characterCode);
        }
    }
}
