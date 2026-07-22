using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Services
{
    public class RescueCalculationModifierService : IRescueCalculationModifierService
    {
        public IReadOnlyList<CardModifierViewModel> GetFabCards()
        {
            return
            [
                new CardModifierViewModel("remote-control-hover-camera", "Remote Control Hover Camera"),
                new CardModifierViewModel("personal-hoverjet", "Personal Hoverjet"),
                new CardModifierViewModel("underwater-sealing-unit", "Underwater Sealing Unit"),
                new CardModifierViewModel("astronaut-spacewalk", "Astronaut Spacewalk")
            ];
        }

        public IReadOnlyList<CardModifierViewModel> GetEventCards()
        {
            return
            [
                new CardModifierViewModel("the-hood-interferes", "The Hood Interferes")
            ];
        }
    }
}
