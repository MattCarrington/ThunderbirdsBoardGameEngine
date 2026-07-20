using ClosedXML.Excel;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Compilation;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Interfaces;
using ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Readers;

namespace ThunderbirdsBoardGameEngine.ReferenceData.Compiler.Source
{
    public sealed class ExcelReferenceDataSource : IReferenceDataSource
    {
        private readonly string _path;
        private readonly ExcelDisasterCardReader _disasterReader = new();
        private readonly ExcelLocationReader _locationReader = new();
        private readonly ExcelCharacterReader _characterReader = new();
        private readonly ExcelThunderbirdReader _thunderbirdReader = new();
        private readonly ExcelPodVehicleReader _podVehicleReader = new();
        private readonly ExcelMapEdgeReader _mapEdgeReader = new();
        private readonly ExcelFabCardReader _fabCardReader = new();
        private readonly ExcelEventCardReader _eventCardReader = new();

        public ExcelReferenceDataSource(string path)
        {
            _path = path;
        }

        public CompilationContext Load()
        {
            using var workbook = new XLWorkbook(_path);

            var disasters = _disasterReader.ReadFrom(GetRequiredWorksheet(workbook, "Disaster Cards"));
            var locations = _locationReader.ReadFrom(GetRequiredWorksheet(workbook, "Locations"));
            var characters = _characterReader.ReadFrom(GetRequiredWorksheet(workbook, "Characters"));
            var thunderbirds = _thunderbirdReader.ReadFrom(GetRequiredWorksheet(workbook, "Thunderbirds"));
            var podVehicles = _podVehicleReader.ReadFrom(GetRequiredWorksheet(workbook, "Pod Vehicles"));
            var mapEdges = _mapEdgeReader.ReadFrom(GetRequiredWorksheet(workbook, "Map Edges"));
            var fabCards = _fabCardReader.ReadFrom(GetRequiredWorksheet(workbook, "F.A.B. Cards"));
            var eventCards = _eventCardReader.ReadFrom(GetRequiredWorksheet(workbook, "Event Cards"));

            return new CompilationContext
            {
                Disasters = disasters,
                Locations = locations,
                Characters = characters,
                Thunderbirds = thunderbirds,
                PodVehicles = podVehicles,
                MapEdges = mapEdges,
                FabCards = fabCards,
                EventCards = eventCards
            };
        }

        private static IXLWorksheet GetRequiredWorksheet(
            IXLWorkbook workbook,
            string worksheetName)
        {
            if (workbook.TryGetWorksheet(worksheetName, out var worksheet))
            {
                return worksheet;
            }

            throw new ReferenceDataCompilationException(
                $"Required worksheet '{worksheetName}' was not found.");
        }
    }
}
