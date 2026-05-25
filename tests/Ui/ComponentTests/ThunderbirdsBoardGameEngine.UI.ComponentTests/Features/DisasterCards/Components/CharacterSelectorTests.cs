using Bunit;
using ThunderbirdsBoardGameEngine.UI.Features.DisasterCards.Components;
using ThunderbirdsBoardGameEngine.UI.ViewModels;
using Xunit;

namespace ThunderbirdsBoardGameEngine.UI.ComponentTests.Features.DisasterCards.Components
{
    public class CharacterSelectorTests : BunitContext
    {
        [Fact]
        public void CharacterSelectorShouldRenderCharacters()
        {
            // Arrange
            var characters = CreateCharacters();


            // Act
            var cut = Render<CharacterSelector>(parameters => parameters
                .Add(p => p.Characters, characters)
            );

            // Assert
            Assert.Contains("Scott", cut.Markup);
            Assert.Contains("Lady Penelope", cut.Markup);
        }

        [Fact]
        public void CharacterSelectorNotifiesParentWhenSelectionChanges()
        {
            // Arrange
            var characters = CreateCharacters();


            string? selectedCharacter = "scott";

            var cut = Render<CharacterSelector>(parameters => parameters
                .Add(p => p.Characters, characters)
                .Add(
                    p => p.SelectedCharacterCodeChanged,
                    value => selectedCharacter = value));

            // Act            
            cut.Find("#characterSelect").Change("lady-penelope");

            // Assert
            Assert.Equal("lady-penelope", selectedCharacter);
        }

        private static IReadOnlyList<CharacterViewModel> CreateCharacters()
        {
            return
            [
                new CharacterViewModel( Key: "scott", DisplayName: "Scott"),
                new CharacterViewModel( Key: "lady-penelope", DisplayName: "Lady Penelope"),
            ];
        }
    }
}
