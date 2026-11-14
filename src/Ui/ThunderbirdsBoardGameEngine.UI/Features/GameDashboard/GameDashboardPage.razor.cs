using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;

public partial class GameDashboardPage
{
    [Parameter]
    public Guid GameId { get; set; }

    [Inject]
    public IGameService GameService { get; set; } = null!;

    [Inject]
    public ILogger<GameDashboardPage> Logger { get; set; } = null!;

    [Inject]
    public IMovementClientService MovementClientService { get; set; } = null!;

    private GameDashboardViewModel? _game;
    private bool _isLoading;
    private bool _notFound;
    private bool _failed;
    private ThunderbirdMachinesDashboardViewModel? _selectedMachine;
    private IReadOnlyList<SelectionOptionViewModel> _movementDestinations = [];
    private bool _isLoadingMovementDestinations;
    private bool _isMoving;
    private string? _movementError;
    private ThunderbirdMachinesDashboardViewModel? _transferSource;
    private IReadOnlyList<ThunderbirdMachinesDashboardViewModel> _transferDestinations = [];
    private bool _isTransferring;
    private string? _transferError;
    private long _dashboardGeneration;

    protected override Task OnParametersSetAsync()
    {
        return LoadGameAsync();
    }

    private async Task LoadGameAsync()
    {
        var generation = ++_dashboardGeneration;
        var requestedGameId = GameId;

        ResetMovementState();
        ResetTransferState();

        _game = null;
        _notFound = false;
        _failed = false;
        _isLoading = true;

        try
        {
            var game = await GameService.GetGameAsync(requestedGameId);

            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            _game = game;
            _notFound = _game is null;
        }
        catch (Exception exception)
        {
            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            _failed = true;

            Logger.LogError(
                exception,
                "Unable to load the requested game dashboard.");
        }
        finally
        {
            if (IsCurrentDashboard(generation, requestedGameId))
            {
                _isLoading = false;
            }
        }
    }

    private async Task BeginMoveAsync(ThunderbirdMachinesDashboardViewModel machine)
    {
        var generation = _dashboardGeneration;
        var requestedGameId = GameId;

        _selectedMachine = machine;
        _movementDestinations = [];
        _movementError = null;
        _isLoadingMovementDestinations = true;

        try
        {
            var movementDestinations = await MovementClientService.GetAccessibleLocationsAsync(machine.ThunderbirdCode);

            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            _movementDestinations = movementDestinations;
        }
        catch (Exception exception)
        {
            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            _movementError = "Movement destinations could not be loaded.";

            Logger.LogError(exception, "Unable to load movement destinations.");
        }
        finally
        {
            if (IsCurrentDashboard(generation, requestedGameId))
            {
                _isLoadingMovementDestinations = false;
            }
        }
    }

    private async Task MoveAsync(string destinationCode)
    {
        if (_selectedMachine is null)
        {
            return;
        }

        var generation = _dashboardGeneration;
        var requestedGameId = GameId;

        _movementError = null;
        _isMoving = true;

        try
        {
            var result = await GameService.MoveThunderbirdAsync(
                requestedGameId,
                _selectedMachine.ThunderbirdCode,
                destinationCode);

            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            switch (result.Outcome)
            {
                case ThunderbirdMovementOutcome.Success
                    when result.UpdatedGameState is not null:

                    _game = result.UpdatedGameState;
                    CancelMove();
                    break;

                case ThunderbirdMovementOutcome.Rejected:
                    _movementError = "That move is not permitted.";
                    break;

                case ThunderbirdMovementOutcome.NotFound:
                    _movementError = "The game or Thunderbird Machine could not be found.";
                    break;

                default:
                    _movementError = "The move could not be completed.";
                    break;
            }
        }
        catch (Exception exception)
        {
            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            _movementError = "The move could not be completed. Please try again.";

            Logger.LogError(
                exception,
                "Unable to move the requested Thunderbird Machine.");
        }
        finally
        {
            if (IsCurrentDashboard(generation, requestedGameId))
            {
                _isMoving = false;
            }
        }
    }

    private void CancelMove()
    {
        ResetMovementState();
    }

    private void ResetMovementState()
    {
        _selectedMachine = null;
        _movementDestinations = [];
        _movementError = null;
        _isLoadingMovementDestinations = false;
        _isMoving = false;
    }

    private void BeginTransfer(ThunderbirdMachinesDashboardViewModel source)
    {
        if (_game is null)
        {
            return;
        }

        _transferSource = source;

        _transferDestinations = _game.ThunderbirdMachines
            .Where(machine => machine.ThunderbirdCode != source.ThunderbirdCode
                && machine.LocationCode == source.LocationCode)
            .ToArray();
    }

    private async Task TransferCharactersAsync(CharacterTransferSelection selection)
    {
        if (_transferSource is null)
        {
            return;
        }

        var generation = _dashboardGeneration;
        var requestedGameId = GameId;

        _transferError = null;
        _isTransferring = true;

        try
        {
            var result = await GameService.TransferCharactersAsync(
                requestedGameId,
                selection);

            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            switch (result.Outcome)
            {
                case CharacterTransferOutcome.Success
                    when result.UpdatedGameState is not null:
                    _game = result.UpdatedGameState;
                    CancelTransfer();
                    break;
                case CharacterTransferOutcome.Rejected:
                    _transferError = "That character transfer is not permitted.";
                    break;
                case CharacterTransferOutcome.NotFound:
                    _transferError = "The game, character, or Thunderbird Machine could not be found.";
                    break;
                default:
                    _transferError = "The character transfer could not be completed.";
                    break;
            }
        }
        catch (Exception exception)
        {
            if (!IsCurrentDashboard(generation, requestedGameId))
            {
                return;
            }

            _transferError = "The character transfer could not be completed. Please try again.";

            Logger.LogError(
                exception,
                "Unable to transfer the requested character.");
        }
        finally
        {
            if (IsCurrentDashboard(generation, requestedGameId))
            {
                _isTransferring = false;
            }
        }
    }

    private void ResetTransferState()
    {
        _transferSource = null;
        _transferDestinations = [];
        _transferError = null;
        _isTransferring = false;
    }

    private void CancelTransfer()
    {
        ResetTransferState();
    }

    private bool IsCurrentDashboard(long generation, Guid gameId)
    {
        return generation == _dashboardGeneration
            && gameId == GameId;
    }
}
