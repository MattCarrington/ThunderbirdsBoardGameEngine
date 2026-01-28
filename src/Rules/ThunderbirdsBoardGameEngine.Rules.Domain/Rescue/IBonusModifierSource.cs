namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    public interface IBonusModifierSource
    {
        IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input);
    }
}
