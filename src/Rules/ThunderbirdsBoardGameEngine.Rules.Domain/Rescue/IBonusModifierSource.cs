namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    internal interface IBonusModifierSource
    {
        IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input);
    }
}
