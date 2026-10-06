using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;

namespace ThunderbirdsBoardGameEngine.UI.Pages;

public partial class Home
{
    [Inject]
    public IGameService GameService { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    private string _gameIdentifier = string.Empty;
    private string? _message;
    private bool _isCreating;

    private async Task CreateGameAsync()
    {
        if (_isCreating)
        {
            return;
        }

        _message = null;
        _isCreating = true;

        try
        {
            var gameId = await GameService.CreateGameAsync();

            Navigation.NavigateTo($"/games/{gameId:D}");
        }
        catch (Exception)
        {
            _message = "The game could not be created. Please try again.";
        }
        finally
        {
            _isCreating = false;
        }
    }

    private void ResumeGame()
    {
        _message = null;

        if (!Guid.TryParse(_gameIdentifier, out var gameId) ||
            gameId == Guid.Empty)
        {
            _message = "Enter a valid game identifier.";
            return;
        }

        Navigation.NavigateTo($"/games/{gameId:D}");
    }
}