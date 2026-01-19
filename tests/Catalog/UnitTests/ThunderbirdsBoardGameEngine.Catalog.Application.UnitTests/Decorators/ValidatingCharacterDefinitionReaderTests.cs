using NSubstitute;
using ThunderbirdsBoardGameEngine.Catalog.Application.Exceptions;
using ThunderbirdsBoardGameEngine.Catalog.Application.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers;
using Xunit;

namespace ThunderbirdsBoardGameEngine.Catalog.Application.UnitTests.Decorators
{
    public class ValidatingCharacterDefinitionReaderTests
    {
        [Fact]
        public async Task GetAllAsync_WhenValidCharacters_ReturnsCharacters()
        {
            // Arrange
            var characters = TestCharacters.ValidSix;

            var inner = Substitute.For<ICharacterReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>()).Returns(characters);

            var validator = CreateValidator(inner);

            // Act
            var result = await validator.GetAllAsync(CancellationToken.None);

            // Assert
            Assert.Same(characters, result);

            await inner.Received(1).GetAllAsync(Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task GetAllAsync_WithCancellationToken_Forwarded()
        {
            // Arrange
            var inner = Substitute.For<ICharacterReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                 .Returns(TestCharacters.ValidSix);

            var validator = CreateValidator(inner);

            using var cancellationToken = new CancellationTokenSource();

            // Act
            _ = await validator.GetAllAsync(cancellationToken.Token);

            // Assert
            await inner.Received(1).GetAllAsync(Arg.Is(cancellationToken.Token));
        }

        [Fact]
        public async Task GetAllAsync_WhenCharacterCollectionIsInvalid_ThrowsValidationException()
        {
            // Arrange
            var characters = new List<CharacterDefinition>
            {
                new(Character.Scott),
                new(Character.Virgil)
            };

            var inner = Substitute.For<ICharacterReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                .Returns(characters);

            var validator = CreateValidator(inner);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CharacterDefinitionValidationException>(() => validator.GetAllAsync(CancellationToken.None));
            Assert.Equal(CharacterDefinitionErrorCode.InvalidCharacterCount, exception.ErrorCode);
        }

        [Fact]
        public async Task GetAllAsync_WhenInnerThrowsException_Bubbles()
        {
            // Arrange
            var expectedException = CatalogDataAccessException.DataMissing("characters.json");

            var validator = CreateValidator(expectedException);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<CatalogDataAccessException>(() => validator.GetAllAsync(CancellationToken.None));
            Assert.Same(expectedException, exception);
        }

        [Fact]
        public async Task GetAllAsync_WhenCancelled_ExceptionBubbles()
        {
            // Arrange
            var validator = CreateValidator(new OperationCanceledException());

            using var cancellationToken = new CancellationTokenSource();
            await cancellationToken.CancelAsync();

            // Act & Assert
            await Assert.ThrowsAsync<OperationCanceledException>(async () => await validator.GetAllAsync(cancellationToken.Token));
        }

        private static ValidatingCharacterDefinitionReader CreateValidator(Exception ex)
        {
            var inner = Substitute.For<ICharacterReader>();
            inner.GetAllAsync(Arg.Any<CancellationToken>())
                 .Returns<Task<IReadOnlyList<CharacterDefinition>>>(_ => throw ex);

            return CreateValidator(inner);
        }

        private static ValidatingCharacterDefinitionReader CreateValidator(ICharacterReader inner)
        {
            return new ValidatingCharacterDefinitionReader(inner);
        }
    }
}
