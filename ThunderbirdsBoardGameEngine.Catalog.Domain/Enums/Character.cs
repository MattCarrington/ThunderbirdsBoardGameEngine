using System.ComponentModel.DataAnnotations;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Enums
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
