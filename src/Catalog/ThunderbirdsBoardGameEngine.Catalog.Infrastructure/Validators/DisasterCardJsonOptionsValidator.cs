using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;

internal sealed class DisasterCardJsonOptionsValidator : IValidateOptions<DisasterCardJsonOptions>
{
    private readonly IFileSystem _fileSystem;

    private const string Key = "Catalog:DisasterCards:Json:FilePath";

    public DisasterCardJsonOptionsValidator(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
    }

    public ValidateOptionsResult Validate(string? name, DisasterCardJsonOptions options)
    {
        if (options is null)
        {
            return ValidateOptionsResult.Fail("DisasterCardJsonOptions is required.");
        }

        var path = options.FilePath;

        var failures = JsonFilePathValidator.ValidateJsonFilePath(Key, path, _fileSystem);

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
