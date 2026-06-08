using Microsoft.AspNetCore.Components;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public partial class MovementPage
    {
        [Inject]
        public IThunderbirdMovementOptionsService ThunderbirdService { get; set; } = null!;

        private IReadOnlyList<ThunderbirdMovementOptions> _mobileThunderbirds = Array.Empty<ThunderbirdMovementOptions>();

        private string? ThunderbirdCode { get; set; }

        protected override void OnInitialized()
        {
            _mobileThunderbirds = ThunderbirdService.GetAllMobileVehicles();
        }
    }
}
