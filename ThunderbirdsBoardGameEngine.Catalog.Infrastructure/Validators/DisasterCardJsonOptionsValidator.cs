using Microsoft.Extensions.Options;
using System.IO.Abstractions;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Validators;

internal sealed class DisasterCardJsonOptionsValidator : IValidateOptions<DisasterCardJsonOptions>
{
    private const string Key = "Catalog:DisasterCards:Json:FilePath";
    private readonly IFileSystem _fileSystem;

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

        var failures = new List<string>();

        var path = options.FilePath;

        if (string.IsNullOrWhiteSpace(path))
        {
            failures.Add($"{Key} is required.");
            return ValidateOptionsResult.Fail(failures); // nothing else to validate
        }

        if (_fileSystem.Directory.Exists(path))
        {
            failures.Add($"{Key} must point to a file, not a directory.");
            return ValidateOptionsResult.Fail(failures); // nothing else to validate
        }

        if (!_fileSystem.Path.IsPathFullyQualified(path))
        { 
            failures.Add($"{Key} must be a fully qualified absolute path after normalisation. Path {path}"); 
        }

        var extension = _fileSystem.Path.GetExtension(path);

        if (!string.Equals(extension, ".json", StringComparison.OrdinalIgnoreCase))
        {
            failures.Add($"{Key} must point to a .json extension.");
        }

        if (!_fileSystem.File.Exists(path))
        {
            failures.Add($"{Key} must point to an existing file. File not found at {path}");
        }

        return failures.Count == 0
            ? ValidateOptionsResult.Success
            : ValidateOptionsResult.Fail(failures);
    }
}
