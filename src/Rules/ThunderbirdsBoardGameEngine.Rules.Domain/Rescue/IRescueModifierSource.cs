namespace ThunderbirdsBoardGameEngine.Rules.Domain.Rescue
{
    /// <summary>
    /// Defines a contract for sources that provide rescue bonus modifiers based on calculation input.
    /// </summary>
    /// <remarks>Implementations of this interface can supply one or more modifiers that affect rescue
    /// calculations. This interface is intended for use in systems where multiple sources may contribute to the overall
    /// set of applied modifiers.</remarks>
    public interface IRescueModifierSource
    {
        /// <summary>
        /// Applies all relevant rescue modifiers to the specified calculation input and returns the resulting applied
        /// modifiers.
        /// </summary>
        /// <param name="input">The input data for the rescue calculation. Cannot be null.</param>
        /// <returns>A sequence of applied rescue modifiers that affect the calculation based on the provided input. The sequence
        /// will be empty if no modifiers are applicable.</returns>
        IEnumerable<AppliedRescueModifier> ApplyRescueModifier(RescueCalculationInput input);
    }
}
