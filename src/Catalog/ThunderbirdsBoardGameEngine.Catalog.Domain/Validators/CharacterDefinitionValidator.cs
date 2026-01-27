using ThunderbirdsBoardGameEngine.Catalog.Domain.Entities;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Enums;
using ThunderbirdsBoardGameEngine.Catalog.Domain.Exceptions;

namespace ThunderbirdsBoardGameEngine.Catalog.Domain.Validators
{
    public static class CharacterDefinitionValidator
    {
        private const int ExpectedCharacterDefinitionCount = 6;

        public static void ValidateAll(IEnumerable<CharacterDefinition> characterDefinitions)
        {
            ArgumentNullException.ThrowIfNull(characterDefinitions);

            if (characterDefinitions.Count() != ExpectedCharacterDefinitionCount)
            {
                throw CharacterDefinitionValidationException.InvalidCount();
            }

            var keys = new HashSet<Character>();

            foreach (var characterDefinition in characterDefinitions)
            {
                if (!keys.Add(characterDefinition.Key))
                {
                    throw CharacterDefinitionValidationException.DuplicateKey(characterDefinition.Key);
                }
            }
        }
    }
}
