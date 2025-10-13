using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Repositories;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Factories;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders
{
    public class DisasterCardJsonRepositoryBuilder
    {
        private string _jsonContent = "[]";
        private IFileReader? _fileReader;
        private ILogger<DisasterCardJsonRepository> _logger = NullLogger<DisasterCardJsonRepository>.Instance;
        private string _filePath = "/disastercards.json";
        private IOptionsSnapshot<JsonSerializerOptions> _jsonOptions = JsonOptionsFactory.CreateJsonOptions();

        internal DisasterCardJsonRepositoryBuilder WithLogger(ILogger<DisasterCardJsonRepository> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        internal DisasterCardJsonRepositoryBuilder WithJson(string jsonContent)
        {
            _jsonContent = jsonContent ?? throw new ArgumentNullException(nameof(jsonContent));
            return this;
        }

        internal DisasterCardJsonRepositoryBuilder WithFileReader(IFileReader fileReader)
        {
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            return this;
        }

        internal DisasterCardJsonRepositoryBuilder WithFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
            }

            _filePath = filePath;
            return this;
        }

        internal DisasterCardJsonRepositoryBuilder WithJsonOptions(IOptionsSnapshot<JsonSerializerOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions;
            return this;
        }

        internal DisasterCardJsonRepository Build()
        {
            if (_fileReader is null)
            {
                _fileReader = new FakeFileReader().Add(_filePath, _jsonContent);
            }

            var disasterCardJsonOptions = Options.Create(new DisasterCardJsonOptions { FilePath = _filePath });

            return new DisasterCardJsonRepository(disasterCardJsonOptions, _fileReader, _logger, _jsonOptions);
        }
    }
}
