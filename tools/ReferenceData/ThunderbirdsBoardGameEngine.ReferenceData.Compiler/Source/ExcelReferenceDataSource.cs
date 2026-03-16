using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source.Readers;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source
{
    public sealed class ExcelReferenceDataSource : IReferenceDataSource
    {
        private readonly string _path;
        private readonly ExcelDisasterCardReader _disasterReader = new();
        private readonly ExcelLocationReader _locationReader = new();
        private readonly ExcelCharacterReader _characterReader = new();
        private readonly ExcelThunderbirdReader _thunderbirdReader = new();

        public ExcelReferenceDataSource(string path)
        {
            _path = path;
        }

        public CompilationContext Load()
        {
            using var workbook = new XLWorkbook(_path);

            var disasters = _disasterReader.ReadFrom(workbook.Worksheet("Disaster Cards"));
            var locations = _locationReader.ReadFrom(workbook.Worksheet("Locations"));
            var characters = _characterReader.ReadFrom(workbook.Worksheet("Characters"));
            var thunderbirds = _thunderbirdReader.ReadFrom(workbook.Worksheet("Thunderbirds"));

            return new CompilationContext
            {
                Disasters = disasters,
                Locations = locations,
                Characters = characters,
                Thunderbirds = thunderbirds
            };
        }
    }
}
