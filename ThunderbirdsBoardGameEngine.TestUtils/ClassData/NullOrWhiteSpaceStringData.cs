using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.ClassData
{
    public sealed class NullOrWhiteSpaceStringData : TheoryData<string?>
    {
        public NullOrWhiteSpaceStringData()
        {
            Add(null);
            
            foreach (var item in new WhiteSpaceStringData())
            {
                Add(item);
            }
        }
    }
}
