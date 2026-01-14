using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using System.Text;
using System.Text.Json;
using ThunderbirdsBoardGameEngine.Catalog.Format.Dtos;
using ThunderbirdsBoardGameEngine.Catalog.Format.Manifest;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Configuration;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Interfaces;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Mappers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Readers;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Fakes;
using ThunderbirdsBoardGameEngine.Catalog.Infrastructure.Utilities;
using ThunderbirdsBoardGameEngine.TestUtils.Catalog.Builders;

namespace ThunderbirdsBoardGameEngine.Catalog.Infrastructure.UnitTests.Builders
{
    public class DisasterCardJsonReaderBuilder
    {
        private string _jsonContent = "[]";
        private List<DisasterCardCatalogDto> _dtos;
        private IFileOpener? _fileReader;
        private IJsonStreamValidator? _jsonStreamValidator;
        private IEnvelopeParser? _envelopeParser;
        private IGeneratedContentValidator? _contentValidator;
        private IDisasterCardDeserializer? _deserializer;
        private IDisasterCardMapper? _mapper;
        private ILogger<DisasterCardJsonReader> _logger = NullLogger<DisasterCardJsonReader>.Instance;
        private string _filePath = "/disastercards.json";

        public DisasterCardJsonReaderBuilder()
        {
            var dto = new DisasterCardCatalogDtoBuilder().Build();
            _dtos = [dto];
        }

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

        internal DisasterCardJsonReaderBuilder WithFileOpener(IFileOpener fileReader)
        {
            _fileReader = fileReader ?? throw new ArgumentNullException(nameof(fileReader));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithJsonStreamValidator(IJsonStreamValidator jsonStreamValidator)
        {
            _jsonStreamValidator = jsonStreamValidator ?? throw new ArgumentNullException(nameof(jsonStreamValidator));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithEnvelopeParser(IEnvelopeParser envelopeParser)
        {
            _envelopeParser = envelopeParser ?? throw new ArgumentNullException(nameof(envelopeParser));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithContentValidator(IGeneratedContentValidator validator)
        {
            _contentValidator = validator ?? throw new ArgumentNullException(nameof(validator));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithDeserializer(IDisasterCardDeserializer deserializer)
        {
            _deserializer = deserializer ?? throw new ArgumentNullException(nameof(deserializer));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithMapper(IDisasterCardMapper mapper)
        {
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            return this;
        }

        internal DisasterCardJsonReaderBuilder WithDtos(List<DisasterCardCatalogDto> dtos)
        {
            _dtos = dtos ?? throw new ArgumentNullException(nameof(dtos));
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

        internal DisasterCardJsonReader Build()
        {
            _fileReader ??= new FakeFileOpener().Add(_filePath, _jsonContent);

            var disasterCardJsonOptions = Options.Create(new DisasterCardJsonOptions
            {
                FilePath = _filePath
            });

            _jsonStreamValidator ??= CreateJsonStreamValidator();
            _envelopeParser ??= CreateEnvelopeParser(_dtos.Count);
            _contentValidator ??= Substitute.For<IGeneratedContentValidator>();
            _deserializer ??= CreateDeserializer(_dtos);
            _mapper ??= new DisasterCardMapper();

            return new DisasterCardJsonReader(disasterCardJsonOptions, _fileReader, _jsonStreamValidator, _envelopeParser, _contentValidator, _deserializer, _mapper, _logger);
        }

        private IJsonStreamValidator CreateJsonStreamValidator()
        {
            var content = Encoding.UTF8.GetBytes(_jsonContent);

            var stream = new MemoryStream(content);

            var jsonStreamValidator = Substitute.For<IJsonStreamValidator>();
            jsonStreamValidator.ValidateStreamAsync(Arg.Any<Stream>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(stream);

            return jsonStreamValidator;
        }

        private IEnvelopeParser CreateEnvelopeParser(int itemCount)
        {
            var payload = new Payload<GeneratedCatalogManifest>
            {
                Manifest = new GeneratedCatalogManifest
                {
                    Catalog = "DisasterCards",
                    SchemaVersion = "1.0",
                    ContentVersion = "1.0.0",
                    GeneratedAtUtc = DateTime.UtcNow,
                    ItemCount = itemCount,
                    Checksum = new Checksum
                    {
                        Algorithm = "SHA256",
                        Value = "dummychecksumvalue"
                    },
                    ToolInfo = new ToolInfo
                    {
                        Name = "DisasterCardCatalogGenerator",
                        Version = "1.0.0"
                    }
                },
                RawData = JsonDocument.Parse(_jsonContent).RootElement.Clone()
            };

            var envelopeParser = Substitute.For<IEnvelopeParser>();
            envelopeParser.ReadEnvelopeAsync<GeneratedCatalogManifest>(Arg.Any<Stream>(), Arg.Any<CancellationToken>())
                .Returns(Task.FromResult(payload));

            return envelopeParser;
        }

        private static IDisasterCardDeserializer CreateDeserializer(List<DisasterCardCatalogDto>? list)
        {
            var deserializer = Substitute.For<IDisasterCardDeserializer>();
            deserializer.Deserialize(Arg.Any<JsonElement>())
                .Returns(list ?? []);

            return deserializer;
        }
    }
}
