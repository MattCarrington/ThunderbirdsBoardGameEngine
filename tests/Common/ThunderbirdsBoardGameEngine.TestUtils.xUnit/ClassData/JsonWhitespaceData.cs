using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData
{
    /// <summary>
    /// Provides JSON payload strings that should be treated as empty or whitespace,
    /// including BOM-only and large whitespace-only inputs.
    /// </summary>
    public sealed class JsonWhiteSpaceData : TheoryData<string?>
    {
        /// <inheritdoc />
        public JsonWhiteSpaceData()
        {
            foreach (var item in new WhiteSpaceStringData())
            {
                Add(item);
            }

            // UTF-8 BOM only or BOM + whitespace
            foreach (var item in new BomOnlyData())
            {
                Add(item);
            }

            // Big whitespace-only (exercise peek window)
            Add(new string(' ', 8192));
            Add("\uFEFF" + new string('\n', 8192));
        }
    }
}
