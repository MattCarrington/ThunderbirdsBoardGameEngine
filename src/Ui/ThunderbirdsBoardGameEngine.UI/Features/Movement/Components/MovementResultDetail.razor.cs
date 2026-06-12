using Microsoft.AspNetCore.Components;
using ThunderbirdsBoardGameEngine.UI.Features.Movement.Models;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Components
{
    public partial class MovementResultDetail
    {
        [Parameter]
        public MovementResultViewModel MovementResult { get; set; } = null!;
    }
}
