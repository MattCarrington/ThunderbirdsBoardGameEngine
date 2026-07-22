using ThunderbirdsBoardGameEngine.UI.Features.Movement.Interfaces;
using ThunderbirdsBoardGameEngine.UI.Features.Shared.ViewModels;

namespace ThunderbirdsBoardGameEngine.UI.Features.Movement.Services
{
    public class EventCardMovementService : IEventCardMovementService
    {
        public IReadOnlyList<CardModifierViewModel> GetSpeedModificationEventCards()
        {
            return
            [
                new CardModifierViewModel("attack-of-the-zombites", "Attack of the Zombites"),
                new CardModifierViewModel("usn-sentinel-missile-strike", "USN Sentinel Missile Strike"),
                new CardModifierViewModel("rocket-malfunction", "Rocket Malfunction")
            ];
        }
    }
}
