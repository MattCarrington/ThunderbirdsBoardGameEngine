using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public partial class MovementPage
    {
        [Inject]
        public IThunderbirdMovementOptionsService ThunderbirdService { get; set; } = null!;

        [Inject]
        public IMovementClientService MovementService { get; set; } = null!;

        private IReadOnlyList<ThunderbirdMovementOptions> _mobileThunderbirds = Array.Empty<ThunderbirdMovementOptions>();
        private IReadOnlyList<MovementLocationOptions> _movementLocations = Array.Empty<MovementLocationOptions>();

        private string? _thunderbirdCode = string.Empty;
        private string? _startLocationCode = string.Empty;
        private string? _destinationCode = string.Empty;

        private MovementResultViewModel? _validationResult;
        private bool _isValidating = false;
        private bool _validationFailed;

        private bool IsValidationDisabled =>
            _isValidating
            || string.IsNullOrEmpty(_thunderbirdCode)
            || string.IsNullOrEmpty(_startLocationCode)
            || string.IsNullOrEmpty(_destinationCode);

        protected override void OnInitialized()
        {
            _mobileThunderbirds = ThunderbirdService.GetAllMobileVehicles();
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
            _validationFailed = false;
            _validationResult = null;

            try
            {
                _validationResult = await MovementService.ValidateMovementAsync(_thunderbirdCode!, _startLocationCode!, _destinationCode!);

                _validationFailed = _validationResult is null;
            }
            catch
            {
                _validationFailed = true;
            }
            finally
            {
                _isValidating = false;
            }
        }

        private async Task OnThunderbirdChanged(string? value)
        {
            _thunderbirdCode = value;
            _startLocationCode = string.Empty;
            _destinationCode = string.Empty;
            ClearValidationState();

            if (!string.IsNullOrEmpty(_thunderbirdCode))
            {
                _movementLocations = await MovementService.GetAccessibleLocationsAsync(_thunderbirdCode);
            }
            else
            {
                _movementLocations = Array.Empty<MovementLocationOptions>();
            }
        }

        private void OnStartLocationChanged(string? value)
        {
            _startLocationCode = value;
            ClearValidationState();
        }

        private void OnDestinationChanged(string? value)
        {
            _destinationCode = value;
            ClearValidationState();
        }

        private void ClearValidationState()
        {
            _validationResult = null;
            _validationFailed = false;
        }
    }
}
