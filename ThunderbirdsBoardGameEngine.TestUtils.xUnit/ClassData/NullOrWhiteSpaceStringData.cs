using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData
{
    /// <summary>
    /// Provides null and whitespace-only strings for validating guard clauses and parsing logic.
    /// </summary>
    public sealed class NullOrWhitespaceStringData : TheoryData<string?>
    {
        /// <inheritdoc />
        public NullOrWhitespaceStringData()
        {
            Add(null);

            foreach (var item in new WhitespaceStringData())
            {
                Add(item);
            }
        }
    }
}
