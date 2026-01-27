using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;

namespace ThunderbirdsBoardGameEngine.TestUtils.Catalog.Helpers
{
    /// <summary>
    /// Provides predefined character definitions for use in test scenarios.
    /// </summary>
    public static class TestCharacters
    {

        public static IReadOnlyList<CharacterDefinition> ValidSix =>
        [
            new(Character.Scott, new(RescueType.Air, 2)),
            new(Character.Virgil, new(RescueType.Land, 2)),
            new(Character.John, new(RescueType.Space, 2)),
            new(Character.Gordon, new(RescueType.Sea, 3)),
            new(Character.Alan, new(RescueType.Space, 2)),
            new(Character.LadyPenelope)
        ];
    }
}
