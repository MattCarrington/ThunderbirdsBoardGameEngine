using ThunderbirdsBoardGameEngine.ReferenceData.Core.Enums;

namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue.Modifiers
{
    public interface IFabCardRescueModifierSource : ICardRescueModifierSource
    {
        public RescueType RescueType { get; }

        public int BonusValue { get; }
    }
}
