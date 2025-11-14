using ThunderbirdsBoardGameEngine.Catalog.Contracts.Dtos.V1;
using ThunderbirdsBoardGameEngine.TestUtils.EqualityComparers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.TestUtils.xUnit.Assertions
{
    /// <summary>
    /// Assertion helpers for comparing equality of <see cref="DisasterCardDto"/> instances
    /// using the domain-specific <see cref="DisasterCardDtoEqualityComparer"/>.
    /// </summary>
    public static class DisasterCardDtoAssertions
    {
        private static readonly IEqualityComparer<DisasterCardDto> Comparer =
            DisasterCardDtoEqualityComparer.Instance;

        /// <summary>
        /// Asserts that two <see cref="DisasterCardDto"/> instances are equal
        /// according to the domain equality rules.
        /// </summary>
        public static void AssertEqual(DisasterCardDto expected, DisasterCardDto actual)
        {
            Assert.Equal(expected, actual, Comparer);
        }

        /// <summary>
        /// Asserts that two sequences of <see cref="DisasterCardDto"/> contain
        /// the same elements regardless of order, using the domain equality rules.
        /// </summary>
        public static void AssertOrderInsensitive(IEnumerable<DisasterCardDto> expected, IEnumerable<DisasterCardDto> actual)
        {
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(actual);

            var expectedSorted = expected.OrderBy(e => e.Id).ToList();
            var actualSorted = actual.OrderBy(a => a.Id).ToList();

            Assert.Equal(expectedSorted.Count, actualSorted.Count); // Technically redundant but clearer message on size mismatch
            Assert.Equal(expectedSorted, actualSorted, Comparer);
        }

        /// <summary>
        /// Asserts that two sequences of <see cref="DisasterCardDto"/> contain
        /// the same elements in the same order, using the domain equality rules.
        /// </summary>
        public static void AssertOrderSensitive(IEnumerable<DisasterCardDto> expected, IEnumerable<DisasterCardDto> actual)
        {
            ArgumentNullException.ThrowIfNull(expected);
            ArgumentNullException.ThrowIfNull(actual);

            var expectedList = expected.ToList();
            var actualList = actual.ToList();

            Assert.Equal(expectedList.Count, actualList.Count); // Technically redundant but clearer message on size mismatch
            Assert.Equal(expectedList, actualList, Comparer);
        }        
    }
}
