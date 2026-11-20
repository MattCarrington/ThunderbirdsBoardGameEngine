using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.ReferenceData.Runtime.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.GameDashboard.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Home;

public partial class HomePage
{
    [Inject]
    public IGameService GameService { get; set; } = null!;

    [Inject]
    public ICharacterService CharacterService { get; set; } = null!;

    [Inject]
    public NavigationManager Navigation { get; set; } = null!;

    private IReadOnlyList<SelectionOptionViewModel> _characters = [];
    private bool _isCreateDialogOpen;
    private string _gameIdentifier = string.Empty;
    private bool _isCreating;
    private string? _createError;
    private string? _resumeError;

    private void BeginCreateGame()
    {
        _createError = null;

        _characters = CharacterService.GetAll();

        _isCreateDialogOpen = true;
    }

    private async Task CreateGameAsync(IReadOnlyList<string> turnOrder)
    {
        if (_isCreating)
        {
            return;
        }

        _createError = null;
        _isCreating = true;

        try
        {
            var gameId = await GameService.CreateGameAsync(turnOrder);

            Navigation.NavigateTo($"/games/{gameId:D}");
        }
        catch (Exception)
        {
            _createError = "The game could not be created. Please try again.";
        }
        finally
        {
            _isCreating = false;
        }
    }

    private void ResumeGame()
    {
        _resumeError = null;

        if (!Guid.TryParse(_gameIdentifier, out var gameId)
            || gameId == Guid.Empty)
        {
            _resumeError = "Enter a valid game identifier.";
            return;
        }

        Navigation.NavigateTo($"/games/{gameId:D}");
    }
    private void CancelCreateGame()
    {
        if (_isCreating)
        {
            return;
        }

        _createError = null;
        _isCreateDialogOpen = false;
    }
}