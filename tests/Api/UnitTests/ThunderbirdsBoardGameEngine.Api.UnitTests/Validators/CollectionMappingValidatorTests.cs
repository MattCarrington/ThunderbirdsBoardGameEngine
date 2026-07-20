using ThunderbirdsBoardGameEngine.Api.Exceptions;
using ThunderbirdsBoardGameEngine.Api.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.xUnit.ClassData;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Api.UnitTests.Validators
{
    public class CollectionMappingValidatorTests
    {
        [Fact]
        public void ValidateStringCollection_NullList_ThrowsBadRequestException()
        {
            // Arrange
            IEnumerable<string>? list = null;

            // Act & Assert
            var ex = Assert.Throws<BadRequestException>(() => CollectionMappingValidator.ValidateStringCollection(list, "TestProperty"));
            Assert.Equal("TestProperty cannot be null.", ex.Message);
        }

        [Theory]
        [ClassData(typeof(NullOrWhitespaceStringData))]
        public void ValidateStringCollection_ContainsNullOrWhitespace_ThrowsBadRequestException(string? invalidValue)
        {
            // Arrange
            var list = new List<string> { "valid", invalidValue!, };

            // Act & Assert
            var ex = Assert.Throws<BadRequestException>(() => CollectionMappingValidator.ValidateStringCollection(list, "TestProperty"));
            Assert.Equal("TestProperty cannot contain null or whitespace values.", ex.Message);
        }
    }
}
