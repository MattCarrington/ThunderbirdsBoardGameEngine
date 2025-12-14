using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Enums
{
    /// <summary>
    /// Identifies a playable character in the Thunderbirds game.
    /// </summary>
    public enum Character
    {
        Scott,
        Virgil,
        Alan,
        Gordon,
        John,

        [Display(Name = "Lady Penelope")]
        LadyPenelope,
    }
}
