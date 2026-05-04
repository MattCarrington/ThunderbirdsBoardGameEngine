namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    /// <summary>
    /// Represents a Pod Vehicle code as an immutable value object.
    /// </summary>
    /// <param name="Value">The string value of the Pod Vehicle code.</param>
    public readonly record struct PodVehicleCode
    {
        public string Value { get; }

        public PodVehicleCode(string value)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(value);

            Value = value;
        }

        public override string ToString()
        {
            return Value;
        }
    }
}
