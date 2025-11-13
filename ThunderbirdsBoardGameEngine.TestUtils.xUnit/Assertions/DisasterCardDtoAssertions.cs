using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.EqualityComparers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions
{
    public static class DisasterCardDtoAssertions
    {
        public static void AssertEqual(DisasterCardDto expected, DisasterCardDto actual)
        {
            Assert.Equal(expected, actual, new DisasterCardDtoEqualityComparer());
        }

        public static void AssertOrderInsensitive(IList<DisasterCardDto> expected, IList<DisasterCardDto> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            Assert.True(expected.OrderBy(e => e.Id).SequenceEqual(actual.OrderBy(a => a.Id), new DisasterCardDtoEqualityComparer()));
        }

        public static void AssertOrderSensitive(IList<DisasterCardDto> expected, IList<DisasterCardDto> actual)
        {
            Assert.Equal(expected.Count, actual.Count);
            Assert.Equal(expected, actual, new DisasterCardDtoEqualityComparer());
        }        
    }
}
