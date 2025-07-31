using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.GameData.Domain.Enums
{
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
