FROM mcr.microsoft.com/playwright/dotnet:v1.60.0-noble

ARG BUILD_CONFIGURATION=Release
ENV BUILD_CONFIGURATION=$BUILD_CONFIGURATION

WORKDIR /src

COPY ["nuget.config", "."]
COPY ["Directory.Packages.props", "."]

COPY ["tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj", "tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/"]

RUN --mount=type=secret,id=github_packages_token \
    dotnet nuget update source GitHub \
      --username "MattCarrington" \
      --password "$(cat /run/secrets/github_packages_token)" \
      --store-password-in-clear-text \
    && dotnet restore "tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests.csproj"

COPY . .

WORKDIR /src/tests/SmokeTests/ThunderbirdsBoardGameEngine.SmokeTests

RUN dotnet build "ThunderbirdsBoardGameEngine.SmokeTests.csproj" \
    -c $BUILD_CONFIGURATION \
    --no-restore

ENTRYPOINT ["sh", "-c", "dotnet test ThunderbirdsBoardGameEngine.SmokeTests.csproj --no-build -c \"$BUILD_CONFIGURATION\" -v normal"]