using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Validators;
using ThunderbirdsBoardGameEngine.TestUtils.ReferenceData.Builders;
using Xunit;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.UnitTests.Validators
{
    public class CardUniquenessValidatorTests
    {
        [Fact]
        public void Validate_WhenDuplicateCardCodes_Throws()
        {
            // Arrange
            var validator = new CardUniquenessValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithDisaster("card-1", "Disaster Card Name", "location-1", ("character-1", 1, null))
                .WithEventCard("card-1", "Event Card Name")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
            Assert.Contains("Card codes must be unique", ex.Message);
            Assert.Contains("card-1", ex.Message);
        }

        [Fact]
        public void Validate_WhenDuplicateCardNames_Throws()
        {
            // Arrange
            var validator = new CardUniquenessValidator();

            var snapshot = new ReferenceDataSnapshotBuilder()
                .WithDisaster("disaster-1", "Duplicate Name", "location-1", ("character-1", 1, null))
                .WithEventCard("event-1", "Duplicate Name")
                .Build();

            // Act & Assert
            var ex = Assert.Throws<ReferenceDataCompilationException>(() => validator.Validate(snapshot));
            Assert.Contains("Card names must be unique", ex.Message);
            Assert.Contains("Duplicate Name", ex.Message);
        }
    }
}