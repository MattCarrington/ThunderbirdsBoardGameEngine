using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Services;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

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
    private IReadOnlyList<MovementLocationOptions> _destinations = [];
    private bool _isLoadingDestinations;
    private bool _isMoving;
    private string? _movementError;

    protected override Task OnParametersSetAsync()
    {
        return LoadGameAsync();
    }

    private async Task LoadGameAsync()
    {
        ResetMovementState();

        _game = null;
        _notFound = false;
        _failed = false;
        _isLoading = true;

        try
        {
            _game = await GameService.GetGameAsync(GameId);
            _notFound = _game is null;
        }
        catch (Exception exception)
        {
            _failed = true;

            Logger.LogError(
                exception,
                "Unable to load the requested game dashboard.");
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task BeginMoveAsync(
        ThunderbirdMachinesDashboardViewModel machine)
    {
        _selectedMachine = machine;
        _destinations = [];
        _movementError = null;
        _isLoadingDestinations = true;

        try
        {
            _destinations = await MovementClientService.GetAccessibleLocationsAsync(machine.ThunderbirdCode);
        }
        catch (Exception exception)
        {
            _movementError = "Movement destinations could not be loaded.";

            Logger.LogError(exception, "Unable to load movement destinations.");
        }
        finally
        {
            _isLoadingDestinations = false;
        }
    }

    private async Task MoveAsync(string destinationCode)
    {
        if (_selectedMachine is null)
        {
            return;
        }

        _movementError = null;
        _isMoving = true;

        try
        {
            var result = await GameService.MoveThunderbirdAsync(
                GameId,
                _selectedMachine.ThunderbirdCode,
                destinationCode);

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
            _movementError = "The move could not be completed. Please try again.";

            Logger.LogError(
                exception,
                "Unable to move the requested Thunderbird Machine.");
        }
        finally
        {
            _isMoving = false;
        }
    }

    private void CancelMove()
    {
        ResetMovementState();
    }

    private void ResetMovementState()
    {
        _selectedMachine = null;
        _destinations = [];
        _movementError = null;
        _isLoadingDestinations = false;
        _isMoving = false;
    }
}
