using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Factories;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders
{
    public class DisasterCardJsonReaderBuilder
    {
        private string _jsonContent = "[]";
        private IFileOpener? _fileReader;
        private ILogger<DisasterCardJsonReader> _logger = NullLogger<DisasterCardJsonReader>.Instance;
        private string _filePath = "/disastercards.json";
        private IOptionsMonitor<JsonSerializerOptions> _jsonOptions = JsonOptionsFactory.CreateJsonOptions();

        internal DisasterCardJsonReaderBuilder WithLogger(ILogger<DisasterCardJsonReader> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithJson(string jsonContent)
        {
            _jsonContent = jsonContent ?? throw new ArgumentNullException(nameof(jsonContent));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithFileReader(IFileOpener fileReader)
        {
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithFilePath(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path cannot be null or whitespace.", nameof(filePath));
            }

            _filePath = filePath;
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithJsonOptions(IOptionsMonitor<JsonSerializerOptions> jsonOptions)
        {
            _jsonOptions = jsonOptions;
            return this;
        }

        internal DisasterCardJsonReader Build()
        {
            if (_fileReader is null)
            {
                _fileReader = new FakeFileOpener().Add(_filePath, _jsonContent);
            }

            var disasterCardJsonOptions = Options.Create(new DisasterCardJsonOptions { FilePath = _filePath });

            return new DisasterCardJsonReader(disasterCardJsonOptions, _fileReader, _logger, _jsonOptions);
        }
    }
}
