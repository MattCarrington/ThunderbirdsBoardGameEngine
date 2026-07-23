using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.ReferenceData.Core.Identities;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement
{
    public partial class MovementPage
    {
        [Inject]
        public IThunderbirdMovementOptionsService ThunderbirdService { get; set; } = null!;

        [Inject]
        public IMovementClientService MovementService { get; set; } = null!;

        [Inject]
        public IEventCardMovementService EventCardMovementService { get; set; } = null!;

        private IReadOnlyList<ThunderbirdMovementOptions> _mobileThunderbirds = Array.Empty<ThunderbirdMovementOptions>();
        private IReadOnlyList<MovementLocationOptions> _movementLocations = Array.Empty<MovementLocationOptions>();
        private IReadOnlyList<CardModifierViewModel> _eventCardModifiers = Array.Empty<CardModifierViewModel>();

        private string? _thunderbirdCode = string.Empty;
        private string? _startLocationCode = string.Empty;
        private string? _destinationCode = string.Empty;

        private HashSet<string> _selectedEventCardKeys = new();

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

            var speedModifiers = EventCardMovementService.GetSpeedModificationEventCards() ?? Array.Empty<CardModifierViewModel>();
            var blockedMovementModifiers = EventCardMovementService.GetBlockedMovementEventCards() ?? Array.Empty<CardModifierViewModel>();

            _eventCardModifiers = speedModifiers.Concat(blockedMovementModifiers).ToList();
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
                _validationResult = await MovementService.ValidateMovementAsync(_thunderbirdCode!, _startLocationCode!, _destinationCode!, _selectedEventCardKeys.ToList());

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

        private async Task OnThunderbirdChanged(string? thunderbirdCode)
        {
            _thunderbirdCode = thunderbirdCode;

            _startLocationCode = string.Empty;
            _destinationCode = string.Empty;
            _movementLocations = Array.Empty<MovementLocationOptions>();

            ClearValidationState();

            if (string.IsNullOrWhiteSpace(thunderbirdCode))
            {
                return;
            }

            var requestedThunderbirdCode = thunderbirdCode;

            var locations =
                await MovementService.GetAccessibleLocationsAsync(requestedThunderbirdCode);

            if (_thunderbirdCode != requestedThunderbirdCode)
            {
                return;
            }

            _movementLocations = locations;
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

        private void OnEventCardChanged(EventCardChanged change)
        {
            if (change.Selected)
            {
                _selectedEventCardKeys.Add(change.Key);
            }
            else
            {
                _selectedEventCardKeys.Remove(change.Key);
            }

            ClearValidationState();
        }
    }
}
