using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.GameDashboard;

public partial class GameDashboard
{
    [Parameter]
    public Guid GameId { get; set; }

    [Inject]
    public IGameService GameService { get; set; } = null!;

    [Inject]
    public ILogger<GameDashboard> Logger { get; set; } = null!;

    private GameDashboardViewModel? _game;
    private bool _isLoading;
    private bool _notFound;
    private bool _failed;

    protected override async Task OnParametersSetAsync()
    {
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

            // Deliberately do not include GameId.
            Logger.LogError(
                exception,
                "Unable to load the requested game dashboard.");
        }
        finally
        {
            _isLoading = false;
        }
    }
}