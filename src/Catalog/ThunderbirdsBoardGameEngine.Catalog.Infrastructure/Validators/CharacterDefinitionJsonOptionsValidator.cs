using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators
{
    internal sealed class CharacterDefinitionJsonOptionsValidator : IValidateOptions<CharacterDefinitionJsonOptions>
    {
        private readonly IFileSystem _fileSystem;

        private const string Key = "Catalog:Characters:Json:FilePath";

        public CharacterDefinitionJsonOptionsValidator(IFileSystem fileSystem)
        {
            _fileSystem = fileSystem;
        }

        public ValidateOptionsResult Validate(string? name, CharacterDefinitionJsonOptions options)
        {
            if (options is null)
            {
                return ValidateOptionsResult.Fail("CharacterDefinitionJsonOptions is required.");
            }

            var path = options.FilePath;

            var failures = JsonFilePathValidator.ValidateJsonFilePath(Key, path, _fileSystem);

            return failures.Count == 0
                ? ValidateOptionsResult.Success
                : ValidateOptionsResult.Fail(failures);
        }
    }
}
