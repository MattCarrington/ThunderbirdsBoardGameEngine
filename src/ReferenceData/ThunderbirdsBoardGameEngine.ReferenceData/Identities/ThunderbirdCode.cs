namespace ThunderbirdsBoardGameEngine.ReferenceData.Identities
{
    public sealed record ThunderbirdCode
    {
        public string Value { get; }

        public ThunderbirdCode(string value)
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
