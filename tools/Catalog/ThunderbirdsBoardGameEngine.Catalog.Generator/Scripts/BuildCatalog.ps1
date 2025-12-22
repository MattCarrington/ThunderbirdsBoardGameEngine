$input = "resources\DisasterCards.xlsx"
$output = "output\DisasterCards.json"

dotnet run -c release --project ..\..\ThunderbirdsBoardGameEngine.Catalog.Generator\ThunderbirdsBoardGameEngine.Catalog.Generator.csproj -- `
    -i $input `
    -o $output 