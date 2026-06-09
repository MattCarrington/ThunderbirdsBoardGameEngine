using Microsoft.AspNetCore.Components;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public partial class MovementPage
    {
        [Inject]
        public IThunderbirdMovementOptionsService ThunderbirdService { get; set; } = null!;

        [Inject]
        public IMovementLocationOptionsService LocationService { get; set; } = null!;

        [Inject]
        public IValidateMovementService MovementService { get; set; } = null!;

        private IReadOnlyList<ThunderbirdMovementOptions> _mobileThunderbirds = Array.Empty<ThunderbirdMovementOptions>();
        private IReadOnlyList<MovementLocationOptions> _movementLocations = Array.Empty<MovementLocationOptions>();

        private string? _thunderbirdCode;
        private string? _startLocationCode;
        private string? _destinationCode;

        private MovementResultViewModel? _validationResult;
        private bool _isValidating = false;
        private bool _validationFailed;

        private bool? IsValidationDisabled =>
            _isValidating
            || string.IsNullOrEmpty(_thunderbirdCode)
            || string.IsNullOrEmpty(_startLocationCode)
            || string.IsNullOrEmpty(_destinationCode);

        protected override void OnInitialized()
        {
            _mobileThunderbirds = ThunderbirdService.GetAllMobileVehicles();
            _movementLocations = LocationService.GetAll();
        }

        private async Task ValidateMovement()
        {
            if (string.IsNullOrEmpty(_thunderbirdCode)
            || string.IsNullOrEmpty(_startLocationCode)
            || string.IsNullOrEmpty(_destinationCode))
            {
                return;
            }

            _isValidating = true;

            _validationResult = await MovementService.ValidateMovementAsync(_thunderbirdCode!, _startLocationCode!, _destinationCode!);

            _validationFailed = _validationResult is null;

            _isValidating = false;
        }
    }
}
