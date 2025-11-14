using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData
{
    /// <summary>
    /// Provides JSON payload strings that should be treated as empty or whitespace,
    /// including BOM-only and large whitespace-only inputs.
    /// </summary>
    public sealed class JsonWhitespaceData : TheoryData<string?>
    {
        /// <inheritdoc />
        public JsonWhitespaceData()
        {
            foreach (var item in new WhitespaceStringData())
            {
                Add(item);
            }

            // UTF-8 BOM only or BOM + whitespace
            Add("\uFEFF");
            Add("\uFEFF ");
            Add("\uFEFF\r\n\t  ");

            // Big whitespace-only (exercise peek window)
            Add(new string(' ', 8192));
            Add("\uFEFF" + new string('\n', 8192));
        }
    }
}
