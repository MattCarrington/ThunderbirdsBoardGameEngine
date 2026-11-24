using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public interface IFabCardRescueModifierSource : ICardRescueModifierSource
    {
        public RescueType RescueType { get; }

        public int BonusValue { get; }
    }
}
