namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source
{
    using ClosedXML.Excel;
    using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
    using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source.Readers;

    public sealed class ExcelReferenceDataSource : IReferenceDataSource
    {
        private readonly string _path;
        private readonly ExcelDisasterCardReader _disasterReader = new();
        private readonly ExcelLocationReader _locationReader = new();

        public ExcelReferenceDataSource(string path)
        {
            _path = path;
        }

        public CompilationContext Load()
        {
            using var workbook = new XLWorkbook(_path);

            var disasters = _disasterReader.ReadFrom(workbook.Worksheet("Disaster Cards"));
            var locations = _locationReader.ReadFrom(workbook.Worksheet("Locations"));

            return new CompilationContext
            {
                Disasters = disasters,
                Locations = locations
            };
        }
    }
}
