using Microsoft.JSInterop.Infrastructure;
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
                ThunderbirdMachines: game.ThunderbirdMachines.Select(MapThunderbirds).ToList());
        }

        private ThunderbirdMachinesDashboardViewModel MapThunderbirds(ThunderbirdMachineStateDto dto)
        {
            return new ThunderbirdMachinesDashboardViewModel(
                ThunderbirdCode: dto.ThunderbirdCode,
                ThunderbirdDisplayName: _thunderbirdDefinitionCatalog.GetByCode(new ThunderbirdCode(dto.ThunderbirdCode)).DisplayName,
                LocationCode: dto.LocationCode,
                LocationDisplayName: _locationDefinitionCatalog.GetByCode(new LocationCode(dto.LocationCode)).DisplayName,
                Occupants: dto.OccupantCharacterCodes.Select(MapOccupants).ToList());
        }

        private OccupantsDashboardViewModel MapOccupants(string occupant)
        {
            return new OccupantsDashboardViewModel(
                CharacterCode: occupant,
                CharacterDisplayName: _characterDefinitionCatalog.GetByCode(new CharacterCode(occupant)).DisplayName);
        }
    }
}
