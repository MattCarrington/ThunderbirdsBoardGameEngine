FROM mcr.microsoft.com/playwright/dotnet:v1.57.0-noble

ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=$BUILD_CONFIGURATION

WORKDIR /src

COPY ["Directory.Packages.props", "."]

COPY ["tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj", "tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Client/ThunderbirdsBoardGameEngine.Rules.Client.csproj", "src/Rules/ThunderbirdsBoardGameEngine.Rules.Client/"]
COPY ["src/Common/ThunderbirdsBoardGameEngine.Client.Infrastructure/ThunderbirdsBoardGameEngine.Client.Infrastructure.csproj", "src/Common/ThunderbirdsBoardGameEngine.Client.Infrastructure/"]
COPY ["src/Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/ThunderbirdsBoardGameEngine.Rules.Contracts.csproj", "src/Rules/ThunderbirdsBoardGameEngine.Rules.Contracts/"]
COPY ["tests/Common/ThunderbirdsBoardGameEngine.TestUtils/ThunderbirdsBoardGameEngine.TestUtils.csproj", "tests/Common/ThunderbirdsBoardGameEngine.TestUtils/"]
COPY ["src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData.csproj", "src/ReferenceData/ThunderbirdsBoardGameEngine.ReferenceData/"]

RUN dotnet restore "tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj"

COPY . .

WORKDIR /src/tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests

RUN dotnet build "ThunderbirdsBoardGameEngine.SmokeTests.csproj" \
    -c $BUILD_CONFIGURATION \
    --no-restore

ENTRYPOINT ["sh", "-c", "dotnet test ThunderbirdsBoardGameEngine.SmokeTests.csproj --no-build -c \"$BUILD_CONFIGURATION\" -v normal"]